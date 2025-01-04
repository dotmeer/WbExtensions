﻿using System;
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
using WbExtensions.Domain.Telegram;
using WbExtensions.Infrastructure.Telegram.Settings;

namespace WbExtensions.Infrastructure.Telegram.Implementations;

internal sealed class TelegramService : ITelegramService, IInitializer
{
    private readonly TelegramBotSettings _settings;
    private readonly ILogger<TelegramService> _logger;
    private readonly ITelegramUserRepository _repository;

    private TelegramBotClient _botClient = default!;
    private bool _inited = false;

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
        var telegramUsers = await _repository.GetAsync(cancellationToken);
        foreach (var user in telegramUsers.Where(u => u.Role > Role.Unknown))
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

        switch (user.Role)
        {
            case Role.Admin:
                switch (message.Text)
                {
                    case "Список пользователей":
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

                    case "Запросы на доступ":
                        var notAllowedUsersMarkup = new InlineKeyboardMarkup();
                        foreach (var notAllowedUser in telegramUsers.Where(u => u.Role == Role.Unknown))
                        {
                            notAllowedUsersMarkup.AddButton(notAllowedUser.UserName!, notAllowedUser.UserId.ToString());
                        }

                        await _botClient.SendMessage(
                            message.Chat,
                            notAllowedUsersMarkup.InlineKeyboard.Any()
                                ? "Список пользователей, ожидающих доступ"
                                : "Нет пользователей, ожидающих доступ",
                            replyMarkup: notAllowedUsersMarkup,
                            cancellationToken: cancellationToken);
                        break;

                    case "Активные пользователи":
                        var allowedUsersMarkup = new InlineKeyboardMarkup();
                        foreach (var allowedUser in telegramUsers.Where(u => u.Role == Role.Keeper))
                        {
                            allowedUsersMarkup.AddButton(allowedUser.UserName!, allowedUser.UserId.ToString());
                        }

                        await _botClient.SendMessage(
                            message.Chat,
                            allowedUsersMarkup.InlineKeyboard.Any()
                                ? "Список пользователей, имеющих доступ"
                                : "Нет пользователей с доступом",
                            replyMarkup: allowedUsersMarkup,
                            cancellationToken: cancellationToken);
                        break;

                    default:
                        await _botClient.SendMessage(
                            message.Chat,
                            "Неизвестная команда",
                            replyMarkup: new ReplyKeyboardMarkup(true)
                                .AddButton("Список пользователей")
                                .AddButton("Запросы на доступ")
                                .AddButton("Активные пользователи"),
                            cancellationToken: cancellationToken);
                        break;
                }
                break;

            case Role.Unknown:
                await _botClient.SendMessage(
                    message.Chat,
                    "Доступ запрещен",
                    replyMarkup: new InlineKeyboardMarkup()
                        .AddButton("Запросить доступ у администратора", user.UserId.ToString()), 
                    cancellationToken: cancellationToken);
                break;
        }
    }

    private async Task ProcessCallbackQuery(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(callbackQuery.Message!.Chat, cancellationToken);

        switch (user.Role)
        {
            case Role.Admin:
                switch (callbackQuery.Message!.Text)
                {
                    case "Список пользователей, ожидающих доступ":
                    case "Пользователь запрашивает доступ":
                        var toAllowUserId = long.Parse(callbackQuery.Data!);
                        var allowedUser = await _repository.FindAsync(toAllowUserId, cancellationToken);
                        await _repository.AllowUserAsync(toAllowUserId, cancellationToken);

                        await _botClient.DeleteMessage(
                            callbackQuery.Message!.Chat,
                            callbackQuery.Message!.MessageId,
                            cancellationToken);

                        await _botClient.SendMessage(
                            callbackQuery.Message!.Chat,
                            $"Пользователю {allowedUser?.UserName} добавлен доступ.",
                            cancellationToken: cancellationToken);

                        await _botClient.SendMessage(
                            new ChatId(toAllowUserId),
                            "Доступ получен",
                            cancellationToken: cancellationToken);
                        break;

                    case "Список пользователей, имеющих доступ":
                        var toDisallowUserId = long.Parse(callbackQuery.Data!);
                        var disallowedUser = await _repository.FindAsync(toDisallowUserId, cancellationToken);
                        await _repository.DeleteAsync(toDisallowUserId, cancellationToken);

                        await _botClient.DeleteMessage(
                            callbackQuery.Message!.Chat,
                            callbackQuery.Message!.MessageId,
                            cancellationToken);

                        await _botClient.SendMessage(
                            callbackQuery.Message!.Chat,
                            $"Доступ для пользователя {disallowedUser?.UserName} удален.",
                            cancellationToken: cancellationToken);
                        break;
                }
                break;

            case Role.Unknown:
                switch (callbackQuery.Message!.Text)
                {
                    case "Доступ запрещен":
                        var users = await _repository.GetAsync(cancellationToken);
                        var askingUser = users.First(u => u.UserId == long.Parse(callbackQuery.Data!));
                        
                        await _botClient.DeleteMessage(
                            callbackQuery.Message!.Chat,
                            callbackQuery.Message!.MessageId,
                            cancellationToken);

                        foreach (var admin in users.Where(u => u.Role == Role.Admin))
                        {
                            await _botClient.SendMessage(
                                new ChatId(admin.UserId),
                                "Пользователь запрашивает доступ",
                                replyMarkup: new InlineKeyboardMarkup()
                                    .AddButton(
                                        $"Выдать пользователю {askingUser.UserName} доступ",
                                        askingUser.UserId.ToString()),
                                cancellationToken: cancellationToken);
                        }

                        break;
                }
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
}

// TODO: тексты в константы