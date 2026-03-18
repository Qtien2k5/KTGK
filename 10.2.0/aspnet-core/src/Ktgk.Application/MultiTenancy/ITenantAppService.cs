using Abp.Application.Services;
using Ktgk.MultiTenancy.Dto;

namespace Ktgk.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

