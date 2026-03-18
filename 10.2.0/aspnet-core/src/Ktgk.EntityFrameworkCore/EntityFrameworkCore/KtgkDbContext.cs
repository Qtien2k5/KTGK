using Abp.Zero.EntityFrameworkCore;
using Ktgk.Authorization.Roles;
using Ktgk.Authorization.Users;
using Ktgk.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace Ktgk.EntityFrameworkCore;

public class KtgkDbContext : AbpZeroDbContext<Tenant, Role, User, KtgkDbContext>
{
    /* Define a DbSet for each entity of the application */

    public KtgkDbContext(DbContextOptions<KtgkDbContext> options)
        : base(options)
    {
    }
}
