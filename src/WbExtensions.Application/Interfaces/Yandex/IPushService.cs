using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Domain.Alice.Push;

namespace WbExtensions.Application.Interfaces.Yandex;

public interface IPushService
{
    Task PushAsync(PushRequest request, CancellationToken cancellationToken);
}