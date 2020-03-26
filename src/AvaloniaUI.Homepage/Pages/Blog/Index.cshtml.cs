using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUI.Homepage.Models;
using Markdig.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Westwind.AspNetCore.Markdown;

namespace AvaloniaUI.Homepage.Pages.Blog
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly MarkdownConfiguration _markdownConfig;

        public IndexModel(
            IWebHostEnvironment env,
            MarkdownConfiguration markdownConfig)
        {
            _env = env;
            _markdownConfig = markdownConfig;
        }

        public IReadOnlyList<BlogPostSummary>? Posts { get; private set; }

        public async Task OnGet()
        {
            var result = new List<BlogPostSummary>();

            foreach (var folder in _markdownConfig.MarkdownProcessingFolders)
            {
                var folderPath = Path.Combine(_env.WebRootPath, folder.RelativePath.Trim('/', '\\'));

                foreach (var path in Directory.EnumerateFiles(
                    folderPath,
                    "*.md",
                    SearchOption.AllDirectories))
                {
                    var post = await ParseMarkdown(path);

                    if (post != null)
                    {
                        post.Url = Path.Combine(folder.RelativePath, Path.GetFileName(path));
                        result.Add(post);
                    }
                }
            }

            Posts = result.OrderByDescending(x => x.Date).ToList();
        }

        private async Task<BlogPostSummary?> ParseMarkdown(string path)
        {
            using var s = new FileStream(path, FileMode.Open);
            using var r = new StreamReader(s);
            var result = new BlogPostSummary();
            var line = await r.ReadLineAsync();

            if (line is null)
            {
                return null;
            }

            if (line.StartsWith("---"))
            {
                if (!await ParseFrontMatter(r, result))
                {
                    return null;
                }
            }

            line = await r.ReadLineAsync();

            var excerpt = new StringBuilder();

            while (line != null && excerpt.Length < 300)
            {
                if (line.StartsWith("#"))
                {
                    if (result.Title is null && line.StartsWith("# ", StringComparison.InvariantCulture))
                    {
                        result.Title = line.Substring(2);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line) &&
                    !line.StartsWith(":::", StringComparison.InvariantCulture) &&
                    !line.StartsWith("```", StringComparison.InvariantCulture) &&
                    !line.StartsWith("![", StringComparison.InvariantCulture) &&
                    !line.StartsWith("[![", StringComparison.InvariantCulture))
                {
                    excerpt.AppendLine(line);
                }

                line = await r.ReadLineAsync();
            }

            if (excerpt.Length > 0)
            {
                excerpt.Append("...");
                result.Excerpt = excerpt.ToString();
            }

            return result;
        }

        private async Task<bool> ParseFrontMatter(StreamReader r, BlogPostSummary result)
        {
            var line = await r.ReadLineAsync();

            while (true)
            {
                if (line == null)
                {
                    return false;
                }
                else if (line.StartsWith("---", StringComparison.Ordinal))
                {
                    return true;
                }
                else if (line.StartsWith("title: ", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Title = line.Substring(7);
                }
                else if (line.StartsWith("published: ", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Date = DateTimeOffset.Parse(line.Substring(11));
                }

                line = await r.ReadLineAsync();
            }
        }
    }
}
