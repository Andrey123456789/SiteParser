using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using TestTask2.Extensions;
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
        /// <param name="pdp">Parse domain params</param>
        /// <returns></returns>
        public static IEnumerable<Product> GetProducts(this HtmlDocument doc, ParseDomainParams pdp)
        {
            if (pdp.SearchPriceKind == SearchPriceKind.spkInner)
                return doc.GetProductsInner(pdp);
            else
                return doc.DocumentNode.GetProductsOuter(pdp);
        }

        /// <summary>
        /// Gets all products from page using SearchPriceKind.spkOuter
        /// </summary>
        /// <param name="node">node</param>
        /// <param name="pdp">parse params</param>
        /// <returns></returns>
        private static IEnumerable<Product> GetProductsOuter(this HtmlNode node, ParseDomainParams pdp)
        {
            HashSet<Product> res = new HashSet<Product>();
            if (node.Name == "script")
                return res;
            int ocCount = GetNodePriceCount(node, pdp);

            if (ocCount > 1)
            {
                if (node.ChildNodes.Count > 1)
                {
                    List<Task> tasks = new List<Task>();
                    object lockObj = new object();
                    foreach (var child in node.ChildNodes)
                    {
                        Task t = new Task(() =>
                        {
                            var childRes = GetProductsOuter(child, pdp);

                            foreach (var p in childRes)
                            {
                                res.LAdd(p, lockObj);
                            }
                        });
                        t.Start();
                        tasks.Add(t);
                    }
                    Task.WaitAll(tasks.ToArray());
                }
                else
                {
                    foreach (var p in GetProductsOuter(node.FirstChild, pdp))
                    {
                        res.Add(p);
                    }
                }
            }
            else if (ocCount == 1)
            {
                //Creating single product:
                var urls = node.GetImageURLs();

                if (TryGetSingleOccurance(node, pdp, out CurrencyRegexes currency, out Regex regex))
                {
                    var product = BuildProduct(node, pdp, regex, urls, currency);
                    if (product != null)
                        res.Add(product);
                }
            }

            return res;
        }

        /// <summary>
        /// Gets all products from page using SearchPriceKind.spkInner
        /// </summary>
        /// <param name="node">node</param>
        /// <param name="pdp">parse params</param>
        /// <returns></returns>
        private static IEnumerable<Product> GetProductsInner(this HtmlDocument doc, ParseDomainParams pdp)
        {
            HashSet<Product> products = new HashSet<Product>();

            //Seaches nodes which contain attribute("грн" for instance) only once
            HashSet<NodeCurrencyInfo> currencyNodeInfos = GetInnerNodesContainingSinglePrice(doc.DocumentNode, pdp);

            foreach (var cni in currencyNodeInfos)
            {
                var node = cni.Node;
                IEnumerable<string> urls;

                //Moving each node from child to parent, until al least one picture is found.
                //If node contains more than one price - cycle is aborted.
                do
                {
                    urls = node.GetImageURLs();

                    //if in one segment there is more than one price or no prices - node doesn't contain product
                    if (GetNodePriceCount(node, pdp) != 1)
                    {
                        break;
                    }

                    if (urls.Count() > 0)
                    {
                        products.Add(BuildProduct(node, pdp, cni.CurrencyRegex, urls, cni.Currency));

                        break;
                    }

                    node = node.ParentNode;
                } while (node != null);
            }
            return products;
        }

        /// <summary>
        /// Get product from node data
        /// </summary>
        /// <param name="node">node</param>
        /// <param name="pdp">Parse params</param>
        /// <param name="curRegex">currency Regex</param>
        /// <param name="urls">urls of images</param>
        /// <returns></returns>
        private static Product BuildProduct(HtmlNode node, ParseDomainParams pdp, Regex curRegex, IEnumerable<string> urls, Currency currency)
        {
            //Parsing price value from node
            string price = FindPrice(node, curRegex, pdp);

            HashSet<Image> images = new HashSet<Image>();
            foreach (var u in urls)
            {
                var img = StringUtils.ImageRegexes.BuildImage(u, pdp.Domain);
                if(img!=null)
                images.Add(img);
            }

            string description = node.GetProductDescription(curRegex, pdp);

            if (description != "")
            {
                return new Product(pdp.Domain, description, Decimal.Parse(price,System.Globalization.NumberStyles.Currency), images, currency);
            }
            return null;
        }

        /// <summary>
        /// Gets the longest value of InnerText of object which doesn't contain children with non-empty innerText
        /// </summary>
        /// <param name="node">node</param>
        /// <param name="pdp">node</param>
        /// <param name="curRegex">Currency Regex</param>
        /// <returns>The longest inner text</returns>
        private static string GetLongestInnerText(this HtmlNode node, ParseDomainParams pdp, Regex curRegex)
        {
            var sLongest = "";

            foreach (var child in node.ChildNodes)
            {
                var s = child.GetLongestInnerText(pdp, curRegex);
                if (s.Length > sLongest.Length)
                {
                    sLongest = s;
                }
            }

            if (sLongest.Length == 0)
            {
                var res = node.InnerText;
                int priceCount = curRegex.Matches(res).Count;

                if (priceCount > 1) return "";

                if (priceCount == 1)
                {
                    var replace = curRegex.Match(res).Value;
                    if (replace == "") return "";
                    res = res.Replace(replace, "");
                }

                return res;
            }
            return sLongest;
        }

        /// <summary>
        /// Get all nodes containing price attribute ("грн" for instance) only once.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static HashSet<NodeCurrencyInfo> GetInnerNodesContainingSinglePrice(HtmlNode node, ParseDomainParams pdp)
        {
            HashSet<NodeCurrencyInfo> res = new HashSet<NodeCurrencyInfo>();

            if (node.Name == "script")
                return res;
            List<Task> tasks = new List<Task>();
            object lockObj = new object();
            int matchesCount = TryGetAnyOccurance(node, pdp, out CurrencyRegexes currency, out Regex regex);
            if (matchesCount>0)
            {
                foreach (var child in node.ChildNodes)
                {
                    Task t = new Task(() =>
                    {
                        HashSet<NodeCurrencyInfo> currencyInfos = GetInnerNodesContainingSinglePrice(child, pdp);
                        foreach (var c in currencyInfos)
                        {
                            res.LAdd(c, lockObj);
                        }
                    });
                    t.Start();
                    tasks.Add(t);
                }

                Task.WaitAll(tasks.ToArray());

                if (res.Count() == 0 && matchesCount==1)
                {
                    res.Add(new NodeCurrencyInfo(node, currency, regex));
                }
            }
            return res;
        }

        /// <summary>
        /// Checks if the attribute ("грн" for instance) is contained only once in the node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool TryGetSingleOccurance(HtmlNode node, ParseDomainParams pdp, out CurrencyRegexes currency, out Regex regex)
        {
            regex = null;
            currency = null;

            bool found = false;
            foreach (var cur in pdp.CurRegexes)
                foreach (var reg in cur.Regexes)
                {
                    var matches = reg.Matches(node.InnerText).Count;
                    if (matches > 1) return false;
                    if (matches == 1)
                    {
                        if (found) return false;

                        regex = reg;
                        currency = cur;

                        found = true;
                    }
                }

            return found;
        }

        /// <summary>
        /// Checks if the attribute ("грн" for instance) is contained once or more in the node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static int TryGetAnyOccurance(HtmlNode node, ParseDomainParams pdp, out CurrencyRegexes currency, out Regex regex)
        {
            regex = null;
            currency = null;

            int foundCount = 0;
            foreach (var cur in pdp.CurRegexes)
                foreach (var reg in cur.Regexes)
                {
                    var matches = reg.Matches(node.InnerText).Count;
                    foundCount += matches;
                    if (matches>0)
                    {
                        regex = reg;
                        currency = cur;
                    }
                    if (foundCount > 1) return foundCount;
                }

            return foundCount;
        }

        /// <summary>
        /// Checks number of occurences of the attribute ("грн" for instance) in the node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static int GetNodePriceCount(HtmlNode node, ParseDomainParams pdp)
        {
            int res = 0;
            foreach (var cur in pdp.CurRegexes)
                foreach (var reg in cur.Regexes)
                {
                    res += reg.Matches(node.InnerText).Count;
                }

            return res;
        }

        /// <summary>
        /// Finds price value
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string FindPrice(HtmlNode node, Regex curRegex, ParseDomainParams pdp)
        {
            var v = curRegex.Match(node.InnerText).Value;
            if (v != "")
            {
                var s = pdp.PriceRegex.Match(v).Value;
                s = Currencies.CutPrice(s, pdp.DecimalSeparator);
                if(pdp.DecimalSeparator!="")
                    s=s.Replace(pdp.DecimalSeparator, Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                return s;
            }

            return "";
        }

        private static string GetProductDescription(this HtmlNode node, Regex curRegex, ParseDomainParams pdp)
        {
            string description;
            if (pdp.DescriptionGetKind == DescriptionGetKind.dgkLongest)
            {
                description = node.GetLongestInnerText(pdp, curRegex);
            }
            else //if(pdp.DescriptionGetKind==DescriptionGetKind.dgkFull)
            {
                description = node.InnerText;

                string match = curRegex.Match(description).Value;

                description = description.Replace(match, "");
            }

            foreach (var s in pdp.SkipDecriptionWhenFound)
            {
                if (description.Contains(s)) return "";
            }

            foreach (var s in pdp.ReplaceInDecription)
            {
                description = description.Replace(s, "");
            }

            return description;
        }

        private static IEnumerable<string> GetImageURLs(this HtmlNode node)
        {
            return node.Descendants("img")
                                   .Select(e => e.GetAttributeValue("src", null))
                                   .Where(s => !String.IsNullOrEmpty(s));
        }
    }
}