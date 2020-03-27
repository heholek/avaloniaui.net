using System.Collections.Generic;

namespace AvaloniaUI.Homepage.Models
{
    public class DocsIndexItem
    {
        public string? Url { get; set; }
        public string? Title { get; set; }
        public int Order { get; set; }
        public List<DocsIndexItem>? Children { get; set; }
    }
}
