namespace Portfolio.Models.ViewModels;

public class DashboardViewModel
{
    public int ProjectCount { get; set; }
    public int BlogPostCount { get; set; }
    public int ReviewCount { get; set; }
    public int CertificateCount { get; set; }
    public int AwardCount { get; set; }
    public int UnreadMessagesCount { get; set; }
    public List<ContactMessage> LatestMessages { get; set; } = new();
}
