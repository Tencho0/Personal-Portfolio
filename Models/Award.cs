using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class Award : SortableEntity
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    public string? GivenBy { get; set; }

    [Required]
    public DateTime DateReceived { get; set; }

    public string? Description { get; set; }

    [StringLength(500)]
    public string? ImagePath { get; set; }

    [StringLength(500)]
    [Url]
    public string? Url { get; set; }
}
