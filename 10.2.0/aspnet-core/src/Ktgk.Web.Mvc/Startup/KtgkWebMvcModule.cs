using Abp.Modules;
using Abp.Reflection.Extensions;
using Ktgk.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Ktgk.Web.Startup;

[DependsOn(typeof(KtgkWebCoreModule))]
public class KtgkWebMvcModule : AbpModule
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfigurationRoot _appConfiguration;

    public KtgkWebMvcModule(IWebHostEnvironment env)
    {
        _env = env;
        _appConfiguration = env.GetAppConfiguration();
    }

    public override void PreInitialize()
    {
        Configuration.Navigation.Providers.Add<KtgkNavigationProvider>();
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(KtgkWebMvcModule).GetAssembly());
    }
}
