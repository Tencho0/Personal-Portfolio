using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models.ViewModels;

public class ContactViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<SocialLink> SocialLinks { get; set; } = new();
    public ContactFormModel Form { get; set; } = new();
    public bool MessageSent { get; set; }
}

public class ContactFormModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message is required")]
    [StringLength(5000, ErrorMessage = "Message cannot exceed 5000 characters")]
    public string Message { get; set; } = string.Empty;
}
