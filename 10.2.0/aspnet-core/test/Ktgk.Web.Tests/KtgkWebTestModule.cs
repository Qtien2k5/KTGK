using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Ktgk.EntityFrameworkCore;
using Ktgk.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Ktgk.Web.Tests;

[DependsOn(
    typeof(KtgkWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class KtgkWebTestModule : AbpModule
{
    public KtgkWebTestModule(KtgkEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(KtgkWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(KtgkWebMvcModule).Assembly);
    }
}