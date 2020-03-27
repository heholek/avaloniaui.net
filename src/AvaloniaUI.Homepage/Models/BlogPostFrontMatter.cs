using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaUI.Homepage.Models
{
    public class BlogPostFrontMatter
    {
        public string? Title { get; set; }
        public DateTimeOffset? Published { get; set; }
        public IReadOnlyList<string>? Categories { get; set; }
        public string? Author { get; set; }
        public string? Excerpt { get; set; }

        public string? Category
        {
            get => Categories?.FirstOrDefault();
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Categories = new[] { value };
                }
            }
        }
    }
}
