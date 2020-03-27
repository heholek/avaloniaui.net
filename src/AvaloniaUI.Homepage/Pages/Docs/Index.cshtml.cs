using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaUI.Homepage.Models;
using AvaloniaUI.Homepage.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AvaloniaUI.Homepage.Pages.Docs
{
    public class IndexModel : PageModel
    {
        private const string DocsRelativePath = "docs";
        private readonly IWebHostEnvironment _env;
        private readonly MarkdownDocumentLoader _loader;

        public IndexModel(IWebHostEnvironment env)
        {
            _env = env;
            _loader = new MarkdownDocumentLoader();
        }

        public DocsArticle? Article { get; private set; }
        public List<DocsIndexItem>? Index { get; private set; }

        public async Task<IActionResult> OnGet(string url)
        {
            var docsPath = Path.Combine(_env.WebRootPath, DocsRelativePath);
            var articlePath = Path.Combine(docsPath, url ?? "index");

            if (Directory.Exists(articlePath))
            {
                articlePath = Path.Combine(articlePath, "index.md");
            }
            else if (!articlePath.EndsWith(".md", StringComparison.InvariantCultureIgnoreCase))
            {
                articlePath += ".md";
            }

            Article = await LoadArticle(articlePath);

            if (Article == null)
            {
                return NotFound();
            }

            Index = LoadIndex(docsPath);
            return Page();
        }

        private async Task<DocsArticle?> LoadArticle(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            var article = await _loader.LoadAsync<DocsArticle, DocsArticleFrontMatter>(path);

            if (article == null)
            {
                return null;
            }

            article.Title = article.FrontMatter?.Title ?? "Untitled";

            return article;
        }

        private List<DocsIndexItem> LoadIndex(string path)
        {
            var result = new List<DocsIndexItem>();

            foreach (var filePath in Directory.EnumerateFiles(path, "*.md"))
            {
                var fileName = Path.GetFileName(filePath);

                if (fileName.Equals("index.md", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var frontMatter = LoadFrontMatter(filePath);

                result.Add(new DocsIndexItem
                {
                    Url = Url.Content(filePath),
                    Title = frontMatter?.Title ?? fileName,
                    Order = frontMatter?.Order ?? int.MaxValue,
                });
            }

            foreach (var dirPath in Directory.EnumerateDirectories(path))
            {
                var indexPath = Path.Combine(dirPath, "index.md");
                var frontMatter = LoadFrontMatter(indexPath);

                if (frontMatter is object)
                {
                    var directoryName = Path.GetFileName(path);

                    result.Add(new DocsIndexItem
                    {
                        Url = Url.Content(dirPath),
                        Title = frontMatter.Title ?? directoryName,
                        Order = frontMatter.Order,
                        Children = LoadIndex(dirPath),
                    });
                }
            }

            result.Sort((x, y) => x.Order - y.Order);
            return result;
        }

        private DocsArticleFrontMatter? LoadFrontMatter(string path)
        {
            if (System.IO.File.Exists(path))
            {
                return _loader.LoadFrontMatter<DocsArticleFrontMatter>(path);
            }

            return null;
        }
    }
}
