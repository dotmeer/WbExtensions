using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Domain;

namespace WbExtensions.Application.Interfaces.Database;

public interface IUserInfoRepository
{
    Task<IReadOnlyCollection<UserInfo>> GetAsync(CancellationToken cancellationToken);

    Task UpsertAsync(UserInfo user, CancellationToken cancellationToken);
}