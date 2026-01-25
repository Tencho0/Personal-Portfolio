using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class SiteSetting
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Key { get; set; } = string.Empty;

    public string? Value { get; set; }
}
