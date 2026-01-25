using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class SocialLink : SortableEntity
{
    [Required]
    [StringLength(100)]
    public string Platform { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Url]
    public string Url { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Icon { get; set; }
}
