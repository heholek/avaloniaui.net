using System.IO;
using System.Threading.Tasks;
using AvaloniaUI.Homepage.Models;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.Utilities;

namespace AvaloniaUI.Homepage.Services
{
    public class MarkdownDocumentLoader<TDocument, TFrontMatter>
        where TDocument : class, IMarkdownDocument, new()
        where TFrontMatter : class, IMarkdownFrontMatter, new()
    {
        public async Task<TDocument?> LoadAsync(string path)
        {
            using var s = new FileStream(path, FileMode.Open);
            using var r = new StreamReader(s);
            var line = await r.ReadLineAsync();
            var result = new TDocument();

            if (line is null)
            {
                return null;
            }

            if (line.StartsWith("---"))
            {
                result.FrontMatter = ParseFrontMatter(r);
            }

            result.Markdown = await r.ReadToEndAsync();
            return result;

        }

        private TFrontMatter? ParseFrontMatter(StreamReader r)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .BuildValueDeserializer();
            var parser = new Parser(r);

            parser.Consume<StreamStart>();
            parser.Consume<DocumentStart>();

            return (TFrontMatter?)deserializer.DeserializeValue(
                parser,
                typeof(TFrontMatter),
                new SerializerState(),
                deserializer);
        }
    }
}
