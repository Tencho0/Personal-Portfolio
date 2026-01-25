using Markdig;

namespace Portfolio.Helpers;

public static class MarkdownHelper
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .DisableHtml() // Sanitize HTML to prevent XSS
        .Build();

    public static string ToHtml(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        return Markdown.ToHtml(markdown, Pipeline);
    }
}
