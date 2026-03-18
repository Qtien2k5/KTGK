using Abp.Authorization;
using Ktgk.Authorization.Roles;
using Ktgk.Authorization.Users;

namespace Ktgk.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
