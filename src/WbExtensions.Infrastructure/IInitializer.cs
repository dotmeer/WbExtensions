using System.Threading;
using System.Threading.Tasks;

namespace WbExtensions.Infrastructure;

public interface IInitializer
{
    string Name { get; }

    int Order { get; }

    Task InitAsync(CancellationToken cancellationToken);
}