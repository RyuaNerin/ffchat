using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace RyuaNerin
{
    internal static class LastestRelease
    {
        public static string CheckNewVersion(string owner, string repository)
        {
            try
            {
                var curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                var req = HttpWebRequest.Create("https://api.github.com/repos/{0}/{1}/releases/latest") as HttpWebRequest;
                req.Timeout = 5000;
                req.UserAgent = "Kisbo";
                using (var res = req.GetResponse())
                using (var stream = res.GetResponseStream())
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    string str = reader.ReadToEnd();

                    if (Version.Parse(Regex.Match(str, @"""tag_name""[ \t]*:[ \t]*""([^""]+)\""").Groups[1].Value) > curVersion)
                        return Regex.Match(str, @"""html_url""[ \t]*:[ \t]*""([^""]+)\""").Groups[1].Value;
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
