using System.Linq;
using AngleSharp.Parser.Html;

namespace Mixi2Discord.Lib.Parsers
{
    public class LoginPostKeyParser
    {
        public string Parse(string html)
        {
            var utf8html = ParserUtility.EncodeEuc2Utf8(html);

            var parser = new HtmlParser();
            var document = parser.Parse(utf8html);

            var node = document
                            .QuerySelectorAll("input")
                            .Single(x => x.GetAttribute("name") == "post_key");

            return node.GetAttribute("value");
        }
    }
}
