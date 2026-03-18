using Abp.Authorization;
using Abp.Runtime.Session;
using Ktgk.Configuration.Dto;
using System.Threading.Tasks;

namespace Ktgk.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : KtgkAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
