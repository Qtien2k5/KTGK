using Abp.Application.Services;
using Ktgk.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace Ktgk.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
