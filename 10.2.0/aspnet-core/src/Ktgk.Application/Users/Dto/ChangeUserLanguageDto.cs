using System.ComponentModel.DataAnnotations;

namespace Ktgk.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}