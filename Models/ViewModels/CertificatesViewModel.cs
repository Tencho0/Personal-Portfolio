namespace Portfolio.Models.ViewModels;

public class CertificatesViewModel
{
    public List<Certificate> Certificates { get; set; } = new();
    public List<Award> Awards { get; set; } = new();
    public string ActiveTab { get; set; } = "certificates";
}
