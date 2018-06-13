using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using HtmlAgilityPack;
using TestTask2.Models;
using TestTask2.StringUtils;

namespace TestTask2.AgilityPackClasses
{
    public static class HtmlDocumentExt
    {

        public static IEnumerable<Product> GetProducts(this HtmlDocument doc, string shortDomain, string domain)
        {
            HashSet<Product> products = new HashSet<Product>();
            HashSet<HtmlNode> currencyNodes = GetNodesContainingCurrency(doc.DocumentNode);

            foreach (var n in currencyNodes)
            {
                var node = n;
                IEnumerable<string> urls;
                string price = FindPrice(node);

                do
                {
                    urls = node.Descendants("img")
                                   .Select(e => e.GetAttributeValue("src", null))
                                   .Where(s => !String.IsNullOrEmpty(s));

                    //if in one segment there is more than one price or no prices - node doesn't contain product
                    if (CheckNodeForCurrency(node) !=1)
                    {
                        break;
                    }

                    if (urls.Count() > 0)
                    {
                        HashSet<Image> images = new HashSet<Image>();
                        foreach (var u in urls)
                        {
                            var url = u;
                            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                            {
                                url = domain + url;
                            }

                            var hwr = (HttpWebRequest)WebRequest.Create(uri);
                            var extension = CurrencyRegices.regImageExtension.Match(url); 
                            hwr.Method = "GET";
                            hwr.Accept = "image/"+extension+",image/*";
                            hwr.KeepAlive = false;
                            var resp = hwr.GetResponse();
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

                            images.Add(new Image(outData));

                            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                            //// Check that the remote file was found. The ContentType
                            //// check is performed since a request for a non-existent
                            //// image file might be redirected to a 404-page, which would
                            //// yield the StatusCode "OK", even though the image was not
                            //// found.
                            //if ((response.StatusCode == HttpStatusCode.OK ||
                            //    response.StatusCode == HttpStatusCode.Moved ||
                            //    response.StatusCode == HttpStatusCode.Redirect) &&
                            //    response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                            //{

                            //    // if the remote file was found, download oit
                            //    using (Stream inputStream = response.GetResponseStream())
                            //    {

                            //        List<byte> bufferData = new List<byte>();
                            //        using (var binaryReader = new BinaryReader(inputStream))
                            //        {
                            //            byte[] buffer = binaryReader.ReadBytes(4096);
                            //            bufferData.AddRange(buffer);
                            //        }
                            //        byte[] imageData = bufferData.ToArray();
                            //        images.Add(new Image(imageData));
                            //    }
                            //}
                        }

                        var description = node.GetLongestInnerText();

                        if (description != "") {
                            products.Add(new Product(shortDomain, description , Int32.Parse(price), images));
                        }
                        break;
                    }

                    node = node.ParentNode;
                } while (node.ParentNode != null);
            }
            return products;
        }

        public static string GetLongestInnerText(this HtmlNode node)
        {
            var sLongest = "";

            foreach (var child in node.ChildNodes)
            {
                var s = child.GetLongestInnerText();
                if (s.Length > sLongest.Length)
                {
                    sLongest = s;
                }
            }

            if (sLongest.Length == 0)
            {
                if (CheckNodeForCurrency(node) == 0)
                    return node.InnerText;
                else
                    return "";
            }
            return sLongest;
        }

        private static HashSet<HtmlNode> GetNodesContainingCurrency(HtmlNode node)
        {
            HashSet<HtmlNode> res = new HashSet<HtmlNode>();

            if (node.Name == "script")
                return res;

            if (CheckNodeForCurrency(node)>0)
            {
                foreach (var child in node.ChildNodes)
                {
                    foreach (var c in GetNodesContainingCurrency(child))
                    {
                        res.Add(c);
                    }
                }
                if (res.Count() == 0)
                {
                    res.Add(node);
                }
            }
            return res;
        }

        private static int CheckNodeForCurrency(HtmlNode node)
        {
            int res = 0;

            foreach(var reg in CurrencyRegices.CurRegices)
            {
                res += reg.Matches(node.InnerText).Count;
            }

            return res;
        }


        //private static bool CheckNodeForCurrency(HtmlNode node)
        //{
        //    return CurrencyRegices.CurRegices.Where(r => r.Match(node.InnerText).Success).Count() > 0;
        //}

        private static string FindPrice(HtmlNode node)
        {
            foreach (var reg in CurrencyRegices.CurRegices)
            {
                var v = reg.Match(node.InnerText).Value;
                if (v != "")
                {
                    var s = CurrencyRegices.price.Match(v).Value;
                    return CurrencyRegices.CutPrice(s);
                }
            }
            return "";
        }
    }
}