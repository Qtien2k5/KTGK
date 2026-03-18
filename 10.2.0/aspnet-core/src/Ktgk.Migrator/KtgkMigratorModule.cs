using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Ktgk.Configuration;
using Ktgk.EntityFrameworkCore;
using Ktgk.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace Ktgk.Migrator;

[DependsOn(typeof(KtgkEntityFrameworkModule))]
public class KtgkMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public KtgkMigratorModule(KtgkEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(KtgkMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            KtgkConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(KtgkMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
