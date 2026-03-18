using Ktgk.Roles.Dto;
using System.Collections.Generic;

namespace Ktgk.Web.Models.Common;

public interface IPermissionsEditViewModel
{
    List<FlatPermissionDto> Permissions { get; set; }
}