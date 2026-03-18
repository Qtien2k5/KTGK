using Abp.Application.Services;
using Ktgk.Sessions.Dto;
using System.Threading.Tasks;

namespace Ktgk.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
