using Abp.AspNetCore.Mvc.Authorization;
using Ktgk.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Ktgk.Web.Controllers;

[AbpMvcAuthorize]
public class AboutController : KtgkControllerBase
{
    public ActionResult Index()
    {
        return View();
    }
}
