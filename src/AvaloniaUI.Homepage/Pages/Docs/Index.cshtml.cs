using System;
using System.Collections.Generic;
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
            var articlePath = NormalizePath(Path.Combine(docsPath, url ?? "index"));

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

            Index = LoadIndex(docsPath, articlePath);
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

        private List<DocsIndexItem> LoadIndex(string path, string selectedPath)
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
                    Url = PhysicalPathToContentPath(filePath, false),
                    Title = frontMatter?.Title ?? fileName,
                    Order = frontMatter?.Order ?? int.MaxValue,
                    IsSelected = filePath == selectedPath,
                });
            }

            foreach (var dirPath in Directory.EnumerateDirectories(path))
            {
                var indexPath = Path.Combine(dirPath, "index.md");
                var frontMatter = LoadFrontMatter(indexPath);

                if (frontMatter is object)
                {
                    var directoryName = Path.GetFileName(path);
                    var item = new DocsIndexItem
                    {
                        Url = PhysicalPathToContentPath(dirPath, true),
                        Title = frontMatter.Title ?? directoryName,
                        Order = frontMatter.Order,
                        Children = LoadIndex(dirPath, selectedPath),
                        IsSelected = indexPath == selectedPath,
                    };

                    item.IsExpanded = item.IsSelected || item.Children.Any(x => x.IsExpanded || x.IsSelected);

                    result.Add(item);
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

        private string NormalizePath(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        private string PhysicalPathToContentPath(string physicalPath, bool isDirectory)
        {
            var directory = Path.GetDirectoryName(physicalPath);
            var fileName = Path.GetFileNameWithoutExtension(physicalPath);
            var path = Path.Combine(directory ?? string.Empty, fileName);
            var result = Path.GetRelativePath(_env.WebRootPath, path).Replace('\\', '/');

            if (isDirectory && !result.EndsWith('/'))
            {
                result += '/';
            }

            return Url.Content("~/" + result);
        }
    }
}
