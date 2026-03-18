using Ktgk.Configuration.Dto;
using System.Threading.Tasks;

namespace Ktgk.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
