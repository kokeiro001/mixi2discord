using System.Linq;
using AngleSharp.Parser.Html;

namespace Mixi2Discord.Lib.Parsers
{
    public class AdditionalLoginParser
    {
        public string Parse(string html)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(html);

            var node = document
                            .QuerySelectorAll("input")
                            .Single(x => x.GetAttribute("name") == "additional_auth_data_id");

            return node.GetAttribute("value");
        }
    }
}
