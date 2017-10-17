using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ChainingAssertion;

namespace Mixi2Discord.Lib.Parsers.Tests
{
    [TestClass()]
    public class LoginPostKeyParserTests
    {
        [TestMethod()]
        public void ParseTest()
        {
            // ログイン完了前の「https://mixi.jp/」をダウンロードしてhtmlディレクトリに配置してから実行する
            var html = File.OpenText("html/root.html").ReadToEnd();
            var parser = new LoginPostKeyParser();
            parser.Parse(html).IsNotEmpty();
        }
    }
}