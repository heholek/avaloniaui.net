using System;

namespace AvaloniaUI.Homepage.Models
{
    public class BlogPost
    {
        public string? Url { get; set; }
        public BlogPostFrontMatter? FrontMatter { get; set; }
        public string? Title { get; set; }
        public DateTimeOffset Date { get; set; }
        public string? Markdown { get; set; }
    }
}
