using System.ComponentModel.DataAnnotations;

namespace Ktgk.Configuration.Dto;

public class ChangeUiThemeInput
{
    [Required]
    [StringLength(32)]
    public string Theme { get; set; }
}
