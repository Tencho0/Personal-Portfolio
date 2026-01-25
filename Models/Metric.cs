using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class Metric : SortableEntity
{
    [Required]
    [StringLength(100)]
    public string Label { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Value { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Icon { get; set; }
}
