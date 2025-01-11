using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Telegram;
using WbExtensions.Application.UseCases.GetRooms;
using WbExtensions.Application.UseCases.GetStateInRoom;
using WbExtensions.Domain.Telegram;
using WbExtensions.Infrastructure.Telegram.Settings;

namespace WbExtensions.Infrastructure.Telegram.Implementations;

internal sealed class TelegramService : ITelegramService, IInitializer
{
    private readonly TelegramBotSettings _settings;
    private readonly ILogger<TelegramService> _logger;
    private readonly ITelegramUserRepository _repository;
    private readonly IMediator _mediator;

    private TelegramBotClient _botClient = default!;
    private bool _inited = false;

    public TelegramService(
        TelegramBotSettings settings,
        ILogger<TelegramService> logger,
        ITelegramUserRepository repository,
        IMediator mediator)

    {
        _settings = settings;
        _logger = logger;
        _repository = repository;
        _mediator = mediator;
    }

    public async Task SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        var telegramUsers = await _repository.GetAsync(cancellationToken);
        foreach (var user in telegramUsers.Where(u => u.IsKnown))
        {
            await _botClient.SendMessage(
                new ChatId(user.UserId),
                message,
                cancellationToken: cancellationToken);
        }
    }

    public string Name => "Telegram client";

    public int Order => 100;

    public Task InitAsync(CancellationToken cancellationToken)
    {
        if (!_inited)
        {
            _botClient = new TelegramBotClient(
                _settings.Token,
                cancellationToken: cancellationToken);
            _botClient.OnUpdate += OnUpdateAsync;
            _botClient.OnError += OnErrorAsync;

            _inited = true;
        }

        return Task.CompletedTask;
    }

    private Task OnUpdateAsync(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                return ProcessMessageAsync(update.Message!, _botClient.GlobalCancelToken);

            case UpdateType.CallbackQuery:
                return ProcessCallbackQuery(update.CallbackQuery!, _botClient.GlobalCancelToken);

            default:
                return Task.CompletedTask;
        }
    }

    private Task OnErrorAsync(Exception exception, HandleErrorSource source)
    {
        _logger.LogError(exception, "Ошибка в телеграм-боте, {Source}", source);
        return Task.CompletedTask;
    }

    private async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(message.Chat, cancellationToken);
        var telegramUsers = await _repository.GetAsync(cancellationToken);

        switch (message.Text)
        {
            case Constants.Start:
                await _botClient.SendMessage(
                    message.Chat,
                    "Добро пожаловать!",
                    replyMarkup: GetDefaultMarkup(user),
                    cancellationToken: cancellationToken);
                break;

            case Constants.GetUserList when user.IsAdmin:
                var text = new StringBuilder()
                    .Append("Список пользователей:")
                    .AppendLine();
                foreach (var telegramUser in telegramUsers.OrderByDescending(u => u.Role).ThenBy(u => u.UserName))
                {
                    text.AppendLine($"{telegramUser.UserName} ({telegramUser.Role})");
                }

                await _botClient.SendMessage(
                    message.Chat,
                    text.ToString(),
                    cancellationToken: cancellationToken);
                break;

            case Constants.GetAccessRequests when user.IsAdmin:
                var notAllowedUsersMarkup = new InlineKeyboardMarkup();
                foreach (var notAllowedUser in telegramUsers.Where(u => u.IsUnknown))
                {
                    notAllowedUsersMarkup.AddNewRow(InlineKeyboardButton.WithCallbackData(notAllowedUser.UserName!, notAllowedUser.UserId.ToString()));
                }

                await _botClient.SendMessage(
                    message.Chat,
                    notAllowedUsersMarkup.InlineKeyboard.Any()
                        ? Constants.WaitingAccessUserList
                        : "Нет пользователей, ожидающих доступ",
                    replyMarkup: notAllowedUsersMarkup,
                    cancellationToken: cancellationToken);
                break;

            case Constants.GetAllowedUserList when user.IsAdmin:
                var allowedUsersMarkup = new InlineKeyboardMarkup();
                foreach (var allowedUser in telegramUsers.Where(u => u.IsKeeper))
                {
                    allowedUsersMarkup.AddNewRow(InlineKeyboardButton.WithCallbackData(allowedUser.UserName!, allowedUser.UserId.ToString()));
                }

                await _botClient.SendMessage(
                    message.Chat,
                    allowedUsersMarkup.InlineKeyboard.Any()
                        ? Constants.AllowedUserList
                        : "Нет пользователей с доступом",
                    replyMarkup: allowedUsersMarkup,
                    cancellationToken: cancellationToken);
                break;

            case Constants.GetDevicesStates when user.IsKnown:
                var roomsMarkup = new InlineKeyboardMarkup();
                foreach (var room in await _mediator.Send(new GetRoomsRequest(), cancellationToken))
                {
                    roomsMarkup.AddNewRow(InlineKeyboardButton.WithCallbackData(room));
                }

                await _botClient.SendMessage(
                    message.Chat,
                    Constants.Rooms,
                    replyMarkup: roomsMarkup,
                    cancellationToken: cancellationToken);
                break;

            default:
                await _botClient.SendMessage(
                    message.Chat,
                    user.Role switch
                    {
                        Role.Unknown => Constants.AccessDenied,
                        _ => Constants.UnknownCommand
                    },
                    replyMarkup: GetDefaultMarkup(user),
                    cancellationToken: cancellationToken);
                break;
        }
    }

    private async Task ProcessCallbackQuery(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(callbackQuery.Message!.Chat, cancellationToken);

        await _botClient.DeleteMessage(
            callbackQuery.Message!.Chat,
            callbackQuery.Message!.MessageId,
            cancellationToken);

        switch (callbackQuery.Message!.Text)
        {
            case Constants.WaitingAccessUserList when user.IsAdmin:
            case Constants.UserAccessRequest when user.IsAdmin:
                var toAllowUserId = long.Parse(callbackQuery.Data!);
                var allowedUser = await _repository.FindAsync(toAllowUserId, cancellationToken);
                await _repository.AllowUserAsync(toAllowUserId, cancellationToken);

                await _botClient.SendMessage(
                    callbackQuery.Message!.Chat,
                    $"Пользователю {allowedUser?.UserName} добавлен доступ.",
                    cancellationToken: cancellationToken);

                await _botClient.SendMessage(
                    new ChatId(toAllowUserId),
                    "Доступ получен",
                    cancellationToken: cancellationToken);
                break;

            case Constants.AllowedUserList when user.IsAdmin:
                var toDisallowUserId = long.Parse(callbackQuery.Data!);
                var disallowedUser = await _repository.FindAsync(toDisallowUserId, cancellationToken);
                await _repository.DeleteAsync(toDisallowUserId, cancellationToken);

                await _botClient.SendMessage(
                    callbackQuery.Message!.Chat,
                    $"Доступ для пользователя {disallowedUser?.UserName} удален.",
                    cancellationToken: cancellationToken);
                break;

            case Constants.AccessDenied when user.IsUnknown:
            case Constants.AccessRequest when user.IsUnknown:
                var users = await _repository.GetAsync(cancellationToken);
                var askingUser = users.First(u => u.UserId == long.Parse(callbackQuery.Data!));

                foreach (var admin in users.Where(u => u.IsAdmin))
                {
                    await _botClient.SendMessage(
                        new ChatId(admin.UserId),
                        Constants.UserAccessRequest,
                        replyMarkup: new InlineKeyboardMarkup()
                            .AddButton(
                                $"Выдать пользователю {askingUser.UserName} доступ",
                                askingUser.UserId.ToString()),
                        cancellationToken: cancellationToken);
                }

                break;

            case Constants.Rooms when user.IsKnown:
                await _botClient.SendMessage(
                    callbackQuery.Message!.Chat,
                    await _mediator.Send(new GetStateInRoomRequest(callbackQuery.Data!), cancellationToken),
                    cancellationToken: cancellationToken);
                break;

            default:
                await _botClient.SendMessage(
                    callbackQuery.Message!.Chat,
                    user.Role switch
                    {
                        Role.Unknown => Constants.AccessDenied,
                        _ => Constants.UnknownCommand
                    },
                    replyMarkup: GetDefaultMarkup(user),
                    cancellationToken: cancellationToken);
                break;
        }
    }

    private async Task<TelegramUser> GetUserAsync(Chat chat, CancellationToken cancellationToken)
    {
        var telegramUser = await _repository.FindAsync(chat.Id, cancellationToken);
        
        if (telegramUser is null)
        {
            telegramUser = new TelegramUser
            {
                UserId = chat.Id,
                UserName = chat.Username,
                Role = _settings.Admins.Contains(chat.Username)
                    ? Role.Admin
                    : Role.Unknown
            };

            await _repository.AddAsync(telegramUser, cancellationToken);
        }

        return telegramUser;
    }

    private IReplyMarkup GetDefaultMarkup(TelegramUser user)
        => user.Role switch
        {
            Role.Unknown => new InlineKeyboardMarkup()
                .AddButton(Constants.AccessRequest, user.UserId.ToString()),
            Role.Keeper => new ReplyKeyboardMarkup(true)
                .AddButton(Constants.GetDevicesStates),
            Role.Admin => new ReplyKeyboardMarkup(true)
                .AddNewRow()
                .AddButton(Constants.GetDevicesStates)
                .AddButton(Constants.GetUserList)
                .AddNewRow()
                .AddButton(Constants.GetAccessRequests)
                .AddButton(Constants.GetAllowedUserList),
            _ => new ReplyKeyboardRemove()
        };
}