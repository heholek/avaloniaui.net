using System;

namespace AvaloniaUI.Homepage.Models
{
    public class BlogPostSummary
    {
        public string Url { get; set; }
        public string? Title { get; set; }
        public DateTimeOffset Date { get; set; }
        public string? Excerpt { get; set; }
    }
}
