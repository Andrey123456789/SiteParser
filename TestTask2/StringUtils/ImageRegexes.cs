using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using TestTask2.Models;

namespace TestTask2.StringUtils
{
    public static class ImageRegexes
    {
        public static Regex regImageExtension = new Regex(@"\.[^\.]*$");

        public static string GetImageHTMLType(string url)
        {
            var s = regImageExtension.Match(url).Value;
            s=s.Replace(".", "");
            switch (s)
            {
                case "jpg":
                case "jpeg": return "jpeg";
                case "png": return "png";
                case "svg": return "svg+xml";
                case "gif": return "gif";
                case "tiff": return "tiff";
                default: return "";
            }

        }

        /// <summary>
        /// Builds image from local or global url
        /// </summary>
        /// <param name="url">local or global url</param>
        /// <param name="domain">domain</param>
        /// <returns></returns>
        public static Image BuildImage(string url, string domain)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                url = domain + url;
            }

            var hwr = (HttpWebRequest)WebRequest.Create(uri);
            var extension = ImageRegexes.GetImageHTMLType(url);

            hwr.Method = "GET";
            hwr.Accept = "image/" + extension + ",image/*";
            hwr.KeepAlive = false;
            WebResponse resp=null;
            try{
                resp = hwr.GetResponse();
            }
            catch
            {
                //If any error happens - return null value
                return null;
            }
            var respStream = resp.GetResponseStream();
            var contentLen = resp.ContentLength;
            byte[] outData;
            using (var tempMemStream = new MemoryStream())
            {
                byte[] buffer = new byte[128];
                while (true)
                {
                    int read = respStream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        outData = tempMemStream.ToArray();
                        break;
                    }
                    tempMemStream.Write(buffer, 0, read);
                }
            }

            return new Image(outData, extension);
        }
    }
}