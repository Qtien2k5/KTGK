using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Ktgk.Authorization;

namespace Ktgk;

[DependsOn(
    typeof(KtgkCoreModule),
    typeof(AbpAutoMapperModule))]
public class KtgkApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<KtgkAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(KtgkApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}
