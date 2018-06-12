using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using TestTask2.Extensions;
using TestTask2.Models;
using TestTask2.StringUtils;

namespace TestTask2.AgilityPackClasses
{
    public class ShopWrapper
    {
       
        public const int MaxProducts = 10;

        private string domainName;

        private string shortDomain;

        private object lockObj = new object();

        private HashSet<string> links = new HashSet<string>();

        private HashSet<Product> products;

        private HtmlWeb web = new HtmlWeb();

        public ShopWrapper(string domainName)
        {
            this.domainName = domainName;
            this.shortDomain = GetDomain.GetDomainFromUrl(domainName);
            links.Add(domainName);
        }

        public HashSet<Product> WrapShop()
        {
            products = new HashSet<Product>();
            Wrap(domainName);
            return products;
        }

        private void Wrap(string url)
        {
            if (products.Count > MaxProducts) return;
            Debug.WriteLine("parsing page:" + url);
            var htmlDoc = web.Load(url);

            foreach (var prod in htmlDoc.GetProducts(domainName, shortDomain))
            {
                if (AddIfNotEqual(products, prod))
                {
                    if (products.Count > MaxProducts) return;
                }
            }

            var linkedPages = htmlDoc.DocumentNode.Descendants("a")
                                              .Select(a => a.GetAttributeValue("href", null));

            foreach (var s in linkedPages)
            {
                if (String.IsNullOrEmpty(s) || StringUtils.StringConstants.linksToSkip.Where(x => x.Match(s).Success).Count() > 0) continue;
                Uri uri;
                string domain;
                var link = s;

                if (!Uri.TryCreate(link, UriKind.Absolute, out uri))
                {
                    domain = shortDomain;
                    link = domainName + link;
                }
                else
                {
                    domain = GetDomain.GetDomainFromUrl(uri);
                    if (domain != shortDomain) continue;
                }

                //processing links like http://mysite.com/#somedata
                link = link.Split('#')[0];

                if (links.Contains(link)) continue;
                links.Add(link);

                Wrap(link);

                if (products.Count > MaxProducts) return;

            }
        }

        private static bool AddIfNotEqual(HashSet<Product> products, Product product)
        {
            if (!products.Any(x => x.Description == product.Description && x.Price == product.Price))
            {
                products.Add(product);
                return true;
            }

            return false;
        }

    }
}