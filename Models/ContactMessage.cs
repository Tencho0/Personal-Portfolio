using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class ContactMessage : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(5000)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;
}
