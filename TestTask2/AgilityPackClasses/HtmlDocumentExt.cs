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
        /// <summary>
        /// Gets all products from page
        /// </summary>
        /// <param name="doc">document</param>
        /// <param name="shortDomain">just domain name like domain.com</param>
        /// <param name="domain">full domain name like http://domain.com/ </param>
        /// <returns></returns>
        public static IEnumerable<Product> GetProducts(this HtmlDocument doc, string shortDomain, string domain)
        {

            HashSet<Product> products = new HashSet<Product>();

            //Seaches nodes which contain attribute("грн" for instance) only once
            HashSet<HtmlNode> currencyNodes = GetNodesContainingCurrency(doc.DocumentNode);

            foreach (var n in currencyNodes)
            {
                var node = n;
                IEnumerable<string> urls;
                //parsing price value from node
                string price = FindPrice(node);

                //moving each node from child to parent, until al least one picture is found.
                //if node contains more than one price - cycle is aborted.
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

                            
                        }

                        var description = node.GetLongestInnerText();

                        if (description != "") {
                            products.Add(new Product(shortDomain, description , Int32.Parse(price), images));
                        }
                        break;
                    }

                    node = node.ParentNode;
                } while (node != null);
            }
            return products;
        }

        /// <summary>
        /// Gets the longest value of InnerText of object which doesn't contain children with non-empty innerText
        /// </summary>
        /// <param name="node"></param>
        /// <returns>The longest inner text</returns>
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

        /// <summary>
        /// Get all nodes containing price attribute ("грн" for instance) only once.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks number of occurences of the attribute ("грн" for instance) in the node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static int CheckNodeForCurrency(HtmlNode node)
        {
            int res = 0;

            foreach(var reg in CurrencyRegices.CurRegices)
            {
                res += reg.Matches(node.InnerText).Count;
            }

            return res;
        }

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