using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace vTalkClient.tools
{
    public class MessageProcessor
    {
        public static bool IsURLString(string uriName)
        {
            Uri uriResult;
            return Uri.TryCreate(uriName, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static bool IsImageUrl(string URL)
        {
            var req = (HttpWebRequest)WebRequest.Create(URL);
            req.Method = "HEAD";
            using (var resp = req.GetResponse())
            {
                return resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                           .StartsWith("image/");
            }
        }

        public static string AddLink(string s)
        {
            string regex = @"^((http|https):\/\/[^\s]+)$";
            return Regex.Replace(s, regex, "<a href='$1' target='_blank'>$1</a>");
        }

        public static string Process(string s)
        {
            if (IsURLString(s) && IsImageUrl(s))
            {
                return "<img src=\"" + s + "\" class=\"msg-img\">";
            }
            s = AddLink(s);
            return s;
        }
    }
}
