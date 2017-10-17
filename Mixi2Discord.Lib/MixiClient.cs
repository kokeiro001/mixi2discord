using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Mixi2Discord.Lib.Models;
using Mixi2Discord.Lib.Parsers;

namespace Mixi2Discord.Lib
{
    public class MixiClient
    {
        HttpClient httpClient;
        CookieContainer cookieContainer = new CookieContainer();

        public MixiClient(IWebProxy proxy)
        {
            var handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = cookieContainer,
                UseProxy = proxy != null,
                Proxy = proxy,
            };
            httpClient = new HttpClient(handler);
        }

        public async Task LoginAsync(string email, string password)
        {
            var loginPageHtml = await httpClient.GetStringAsync(@"https://mixi.jp/");
            var postKey = new LoginPostKeyParser().Parse(loginPageHtml);

            var uri = new Uri(@"https://mixi.jp/login.pl?from=login1");

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "email", email },
                { "password", password },
                { "post_key", postKey },
                { "next_url", "/home.pl" },
            });

            var result = await httpClient.PostAsync(uri, content);

            var resultContent = await result.Content.ReadAsStringAsync();
            if (resultContent.Contains("より安全にご利用いただくため、いつもと違う環境からログインする場合、生年月日の確認を行っております。"))
            {
                var additionalResult = await AdditionalLogin(resultContent);
                SetHeader2Cookie(additionalResult);
            }
            else
            {
                SetHeader2Cookie(result);
            }
        }

        private void SetHeader2Cookie(HttpResponseMessage result)
        {
            var setCookie = result.Headers.Single(x => x.Key == "Set-Cookie").Value;

            {
                // セッションキーをCookieに反映する
                var session = setCookie.Single(x => x.StartsWith("session"));
                var sessionValue = session.Split(';').First().Split('=').Last();
                var sessionCookie = new Cookie("session", sessionValue, "/", ".mixi.jp");
                sessionCookie.HttpOnly = true;
                cookieContainer.Add(sessionCookie);
            }
            {
                // stampをCookieに反映する
                var stamp = setCookie.Single(x => x.StartsWith("stamp"));
                var stampValue = stamp.Split(';').First().Split('=').Last();
                var stampCookie = new Cookie("stamp", stampValue, "/", ".mixi.jp");
                stampCookie.HttpOnly = true;
                cookieContainer.Add(stampCookie);
            }
        }

        private async Task<HttpResponseMessage> AdditionalLogin(string html)
        {
            var uri = new Uri(@"https://mixi.jp/login.pl?from=login0");

            var additionalAuthDataId = new AdditionalLoginParser().Parse(html);

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "mode", "additional_auth_post" },
                { "additional_auth_data_id", additionalAuthDataId },
                { "year", "1990" },
                { "month", "4" },
                { "day", "18" },
            });

            httpClient.DefaultRequestHeaders.Date = DateTime.UtcNow.AddHours(9);

            return await httpClient.PostAsync(uri, content);
        }

        public async Task<IEnumerable<VoiceModel>> GetRecentVoicesAsync()
        {
            var url = new Uri(@"http://mixi.jp/recent_voice.pl");
            var html = await httpClient.GetStringAsync(url);

            var parser = new RecentVoicesParser();
            return parser.Parse(html);
        }
    }
}
