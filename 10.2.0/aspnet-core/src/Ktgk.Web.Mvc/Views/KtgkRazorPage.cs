using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Ktgk.Web.Views;

public abstract class KtgkRazorPage<TModel> : AbpRazorPage<TModel>
{
    [RazorInject]
    public IAbpSession AbpSession { get; set; }

    protected KtgkRazorPage()
    {
        LocalizationSourceName = KtgkConsts.LocalizationSourceName;
    }
}
