using System.Threading;
using System.Threading.Tasks;

namespace WbExtensions.Application.Interfaces.Database;

public interface IDatabaseMigrator
{
    Task InitAsync(CancellationToken cancellationToken);
}