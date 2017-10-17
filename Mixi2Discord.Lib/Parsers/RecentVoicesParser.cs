using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Mixi2Discord.Lib.Models;

namespace Mixi2Discord.Lib.Parsers
{
    public class RecentVoicesParser
    {

        public IEnumerable<VoiceModel> Parse(string html)
        {
            var utf8html = ParserUtility.EncodeEuc2Utf8(html);

            var parser = new HtmlParser();
            var document = parser.Parse(utf8html);

            var listAreaNode = document.QuerySelectorAll("ul")
                .Single(x => x.ClassName == "listArea");

            var archiveNodes = listAreaNode
                                .QuerySelectorAll("li")
                                .Where(x => x.ClassName == "archive");

            return archiveNodes.Select(x => ParseArchiveNode(x));
        }

        private VoiceModel ParseArchiveNode(IElement archiveNode)
        {
            // parse nickname
            var nickName = archiveNode
                .QuerySelectorAll("div")
                .Single(x => x.ClassName == "thumbArea")
                .TextContent;

            // parse voice body
            var voiceNode = archiveNode
                .QuerySelectorAll("div")
                .Single(x => x.ClassName == "voiced");

            var voiceSpanText = voiceNode.QuerySelectorAll("span").First().TextContent;

            var spanIdx = voiceNode.TextContent.IndexOf(voiceSpanText);
            var voice = voiceNode.TextContent.Substring(0, spanIdx).Trim();

            // parse post_time
            var postTimeStr = archiveNode
                                .QuerySelectorAll("input")
                                .First(x => x.GetAttribute("name") == "post_time")
                                .GetAttribute("value");
            var postTime = DateTime.ParseExact(postTimeStr, "yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo).AddHours(-9);

            // parse url
            var url = voiceNode
                        .QuerySelectorAll("span")
                        .First()
                        .QuerySelectorAll("a")
                        .First()
                        .GetAttribute("href");

            var voiceModel = new VoiceModel
            {
                NickName = nickName,
                Voice = voice,
                PostTime = postTime,
                Url = url,
                Response = ParseVoiceResponseNode(archiveNode),
            };
            foreach (var item in voiceModel.Response)
            {
                item.Parent = voiceModel;
            }

            return voiceModel;
        }

        private static List<VoiceResponse> ParseVoiceResponseNode(IElement archiveNode)
        {
            var commentNode = archiveNode.QuerySelectorAll("dl").Single(x => x.ClassName == "comment");

            var rowNodes = commentNode
                            .QuerySelectorAll("dd")
                            .Where(x => x.ClassName == "commentRow hrule");

            var list = new List<VoiceResponse>();

            foreach (var rowNode in rowNodes)
            {
                // parse nickname
                var nickName = rowNode.QuerySelectorAll("a").Single(x => x.ClassName == "commentNickname").TextContent;

                // parse voice
                var bodyNode = rowNode.QuerySelectorAll("p").Single(x => x.ClassName == "commentBody");
                var bodySpanText = bodyNode.QuerySelectorAll("span").First().TextContent;
                var spanIdx = bodyNode.TextContent.IndexOf(bodySpanText);
                var voice = bodyNode.TextContent.Substring(0, spanIdx).Trim();

                // parse post date
                var postTimeStr = rowNode.QuerySelectorAll("input")
                    .Single(x => x.GetAttribute("name").Contains("comment_post_time"))
                    .GetAttribute("value");
                var postTime = DateTime.ParseExact(postTimeStr, "yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo).AddHours(-9);

                list.Add(new VoiceResponse
                {
                    NickName = nickName,
                    Voice = voice,
                    PostTime = postTime,
                });
            }

            return list;
        }
    }

    public static class ParserUtility
    {
        public static string EncodeEuc2Utf8(string html)
        {
            Encoding src = Encoding.GetEncoding("euc-jp");
            byte[] temp = src.GetBytes(html);
            return EncodeEuc2Utf8(temp);
        }

        public static string EncodeEuc2Utf8(byte[] srcBytes)
        {
            Encoding src = Encoding.GetEncoding("euc-jp");
            Encoding dest = Encoding.UTF8;
            byte[] utf8tmp = Encoding.Convert(src, dest, srcBytes);
            string utf8html = dest.GetString(utf8tmp);
            return utf8html;
        }

        public static string ByteArray2Euc(byte[] srcBytes)
        {
            Encoding src = Encoding.GetEncoding("euc-jp");
            return src.GetString(srcBytes);
        }
    }
}
