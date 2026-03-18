using Ktgk.Roles.Dto;
using System.Collections.Generic;

namespace Ktgk.Web.Models.Roles;

public class RoleListViewModel
{
    public IReadOnlyList<PermissionDto> Permissions { get; set; }
}
