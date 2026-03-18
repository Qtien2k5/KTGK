using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Ktgk.Controllers
{
    public abstract class KtgkControllerBase : AbpController
    {
        protected KtgkControllerBase()
        {
            LocalizationSourceName = KtgkConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
