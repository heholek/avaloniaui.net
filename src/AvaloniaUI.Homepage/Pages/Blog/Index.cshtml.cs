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
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;
using YamlDotNet.Serialization.Utilities;

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

        public IReadOnlyList<BlogPost>? Posts { get; private set; }

        public async Task OnGet()
        {
            var result = new List<BlogPost>();

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

            Posts = result.OrderByDescending(x => x.FrontMatter!.Published).ToList();
        }

        private async Task<BlogPost?> ParseMarkdown(string path)
        {
            using var s = new FileStream(path, FileMode.Open);
            using var r = new StreamReader(s);
            var result = new BlogPost();
            var line = await r.ReadLineAsync();

            if (line is null)
            {
                return null;
            }

            if (line.StartsWith("---"))
            {
                result.FrontMatter = ParseFrontPatter(r);
            }

            result.Title = result.FrontMatter?.Title;
            result.Date = result.FrontMatter?.Published ?? System.IO.File.GetCreationTimeUtc(path);

            line = await r.ReadLineAsync();

            var excerpt = new StringBuilder();

            while (line != null && excerpt.Length < 300)
            {
                if (line.StartsWith("#"))
                {
                    if (result.Title is null)
                    {
                        result.Title = line.TrimStart('#').Trim();
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
                result.Markdown = excerpt.ToString();
            }

            return result;
        }

        private BlogPostFrontMatter? ParseFrontPatter(StreamReader r)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .BuildValueDeserializer();
            var parser = new Parser(r);

            parser.Consume<StreamStart>();
            parser.Consume<DocumentStart>();

            return (BlogPostFrontMatter?)deserializer.DeserializeValue(
                parser,
                typeof(BlogPostFrontMatter),
                new SerializerState(),
                deserializer);
        }
    }
}
