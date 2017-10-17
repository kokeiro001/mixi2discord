using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ChainingAssertion;

namespace Mixi2Discord.Lib.Parsers.Tests
{
    [TestClass()]
    public class RecentVoicesParserTests
    {
        [TestMethod()]
        public void ParseTest()
        {
            // ログイン完了後の「http://mixi.jp/recent_voice.pl」をダウンロードしてhtmlディレクトリに配置してから実行する
            var bytes = File.ReadAllBytes("html/recent_voice.html");
            var eucHtml = ParserUtility.ByteArray2Euc(bytes);

            var parser = new RecentVoicesParser();
            parser.Parse(eucHtml).IsNotEmpty();
        }
    }
}

