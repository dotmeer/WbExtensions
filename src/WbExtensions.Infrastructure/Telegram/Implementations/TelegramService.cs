using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Telegram;
using WbExtensions.Domain;
using WbExtensions.Infrastructure.Telegram.Settings;

namespace WbExtensions.Infrastructure.Telegram.Implementations;

internal sealed class TelegramService : ITelegramService
{
    private readonly TelegramBotSettings _settings;
    private readonly ILogger<TelegramService> _logger;
    private readonly ITelegramUserRepository _repository;

    private TelegramBotClient _botClient;
    private bool _inited = false;
    private ConcurrentBag<TelegramUser> _telegramUsers = [];

    public TelegramService(
        TelegramBotSettings settings,
        ILogger<TelegramService> logger,
        ITelegramUserRepository repository)

    {
        _settings = settings;
        _logger = logger;
        _repository = repository;
    }

    public async Task SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        foreach (var user in _telegramUsers.Where(u => u.IsAllowed))
        {
            await _botClient.SendMessage(
                new ChatId(user.UserId),
                message,
                replyMarkup: GetRequestMarkup(user),
                cancellationToken: cancellationToken);
        }
    }

    public async Task InitAsync(CancellationToken cancellationToken)
    {
        if (!_inited)
        {
            _botClient = new TelegramBotClient(
                _settings.Token,
                cancellationToken: cancellationToken);
            _botClient.OnUpdate += OnUpdateAsync;
            _botClient.OnError += OnErrorAsync;

            _telegramUsers = new ConcurrentBag<TelegramUser>(
                await _repository.GetAsync(cancellationToken));

            _inited = true;
        }
    }

    private Task OnUpdateAsync(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                return ProcessMessageAsync(update.Message!, _botClient.GlobalCancelToken);

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

        await _botClient.SendMessage(
            message.Chat,
            $"Message: '{message.Text}' from {message.Chat.Username}",
            replyMarkup: GetRequestMarkup(user),
            cancellationToken: cancellationToken);
    }

    private async Task<TelegramUser> GetUserAsync(Chat chat, CancellationToken cancellationToken)
    {
        var telegramUser = _telegramUsers.FirstOrDefault(u => u.UserId == chat.Id);
        
        if (telegramUser is null)
        {
            telegramUser = new TelegramUser
            {
                UserId = chat.Id,
                UserName = chat.Username,
                IsAllowed = _settings.Admins.Contains(chat.Username),
                IsAdmin = _settings.Admins.Contains(chat.Username)
            };

            await _repository.AddAsync(telegramUser, cancellationToken);
            _telegramUsers.Add(telegramUser);
        }

        return telegramUser;
    }

    private IReplyMarkup GetRequestMarkup(TelegramUser telegramUser)
    {
        var replyMarkup = new ReplyKeyboardRemove();

        //if (telegramUser.IsAdmin)
        //{
        //    replyMarkup.AddButton("Управление пользователями", "/users");
        //}

        return replyMarkup;
    }
}