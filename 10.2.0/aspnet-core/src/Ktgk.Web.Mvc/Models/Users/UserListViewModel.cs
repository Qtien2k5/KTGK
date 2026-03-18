using Ktgk.Roles.Dto;
using System.Collections.Generic;

namespace Ktgk.Web.Models.Users;

public class UserListViewModel
{
    public IReadOnlyList<RoleDto> Roles { get; set; }
}
