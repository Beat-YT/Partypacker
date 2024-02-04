using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Partypacker.Core
{
    internal class PartypackServer
    {
        public static string BaseURL =
#if DEBUG
                "https://sparks-staging.coolmath.tech";
#else
                MainWindow.settings.GetValue("Launcher", "apiurl") ?? "https://partypack.mcthe.dev";
#endif

        public static string DashboardURL =
#if DEBUG
                "https://sparks-staging.coolmath.tech";
#else
                MainWindow.settings.GetValue("Launcher", "dashurl") ?? "https://partypack.mcthe.dev";
#endif

        public static KeyValuePair<bool, string> GET(string URL = "/")
        {
            if (URL.StartsWith("/"))
                URL = BaseURL + URL;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Headers.Add("x-sparks-staging", "f3d0e05928afb8257b01477120739a93264c1817edf93999a46af1e226a5e18aee19111578ee73814c7eaffae7b57b50ae0a2902ebd868fe3fbd91b5a25391b46a5b0ce54d2f57d11bf249381370def840d3e6aaf929023892104f701298830478f62297cef250153ad5acc1ec09de494a644206dc275a4692b360315737ab96");
            request.Method = "GET";
            // request.Headers.Add("my-header", "my-value");

            string Response = "";
            try
            {
                var WebResponse = request.GetResponse();

                StreamReader sr = new StreamReader(WebResponse.GetResponseStream());
                Response = sr.ReadToEnd();
                sr.Close();

                return new KeyValuePair<bool, string>(true, Response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new KeyValuePair<bool, string>(false, string.Empty);
            }
        }
        public static KeyValuePair<bool, string> POST(string URL = "/", string Body = "")
        {
            if (URL.StartsWith("/"))
                URL = BaseURL + URL;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "text/plain";
            request.Headers.Add("x-sparks-staging", "f3d0e05928afb8257b01477120739a93264c1817edf93999a46af1e226a5e18aee19111578ee73814c7eaffae7b57b50ae0a2902ebd868fe3fbd91b5a25391b46a5b0ce54d2f57d11bf249381370def840d3e6aaf929023892104f701298830478f62297cef250153ad5acc1ec09de494a644206dc275a4692b360315737ab96");

            string Response = "";
            try
            {
                var WebRequest = request.GetRequestStream();
                WebRequest.Write(Encoding.UTF8.GetBytes(Body), 0, Body.Length);

                var WebResponse = request.GetResponse();

                StreamReader sr = new StreamReader(WebResponse.GetResponseStream());
                Response = sr.ReadToEnd();
                sr.Close();

                return new KeyValuePair<bool, string>(true, Response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new KeyValuePair<bool, string>(false, string.Empty);
            }
        }
    }


}