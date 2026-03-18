using Abp.AspNetCore.Mvc.ViewComponents;

namespace Ktgk.Web.Views;

public abstract class KtgkViewComponent : AbpViewComponent
{
    protected KtgkViewComponent()
    {
        LocalizationSourceName = KtgkConsts.LocalizationSourceName;
    }
}
