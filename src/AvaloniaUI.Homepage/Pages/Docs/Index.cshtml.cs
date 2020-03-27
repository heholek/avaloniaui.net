using System.IO;
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
        private readonly IWebHostEnvironment _env;

        public IndexModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        public DocsArticle Article { get; private set; }

        public async Task<IActionResult> OnGet(string url)
        {
            var localPath = Path.Combine(_env.WebRootPath, "docs", (url ?? "index") + ".md");

            if (!System.IO.File.Exists(localPath))
            {
                return NotFound();
            }

            var loader = new MarkdownDocumentLoader<DocsArticle, DocsArticleFrontMatter>();
            var article = await loader.LoadAsync(localPath);

            if (article == null)
            {
                return NotFound();
            }

            article.Title = article.FrontMatter?.Title ?? "Untitled";

            Article = article;
            return Page();
        }
    }
}
