using Abp.Modules;
using Abp.Reflection.Extensions;
using Ktgk.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Ktgk.Web.Host.Startup
{
    [DependsOn(
       typeof(KtgkWebCoreModule))]
    public class KtgkWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public KtgkWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KtgkWebHostModule).GetAssembly());
        }
    }
}
