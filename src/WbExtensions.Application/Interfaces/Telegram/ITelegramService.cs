using System.Threading;
using System.Threading.Tasks;

namespace WbExtensions.Application.Interfaces.Telegram;

public interface ITelegramService
{
    Task SendMessageAsync(string message, CancellationToken cancellationToken);

    Task InitAsync(CancellationToken cancellationToken);
}