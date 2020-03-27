namespace AvaloniaUI.Homepage.Models
{
    public interface IMarkdownDocument
    {
        string? Url { get; set; }
        IMarkdownFrontMatter? FrontMatter { get; set; }
        string? Markdown { get; set; }
    }
}
