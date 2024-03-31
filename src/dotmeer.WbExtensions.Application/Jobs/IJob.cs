using System.Threading.Tasks;
using System.Threading;

namespace dotmeer.WbExtensions.Application.Jobs;

public interface IJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}