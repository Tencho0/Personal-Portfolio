namespace Portfolio.Models.ViewModels;

public class HomeViewModel
{
    public string ProfileName { get; set; } = string.Empty;
    public string ProfileTitle { get; set; } = string.Empty;
    public string ProfileBio { get; set; } = string.Empty;
    public string ProfileImagePath { get; set; } = string.Empty;
    public List<Metric> Metrics { get; set; } = new();
    public List<TechStackItem> TechStack { get; set; } = new();
    public List<Review> FeaturedReviews { get; set; } = new();
    public List<BlogPost> LatestPosts { get; set; } = new();
}
