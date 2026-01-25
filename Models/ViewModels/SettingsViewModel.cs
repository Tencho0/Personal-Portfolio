using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models.ViewModels;

public class SettingsViewModel
{
    [Required(ErrorMessage = "Profile name is required")]
    [StringLength(200)]
    public string ProfileName { get; set; } = string.Empty;

    [StringLength(200)]
    public string ProfileTitle { get; set; } = string.Empty;

    [StringLength(5000)]
    public string ProfileBio { get; set; } = string.Empty;

    public string? ProfileImagePath { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(200)]
    public string ContactEmail { get; set; } = string.Empty;

    [StringLength(50)]
    public string ContactPhone { get; set; } = string.Empty;
}
