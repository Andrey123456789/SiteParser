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
    /// <summary>
    /// Searches for products in the shop
    /// </summary>
    public class ShopWrapper
    {

        /// <summary>
        /// Number of objects to find
        /// </summary>
        public const int MaxProducts = 10;

        /// <summary>
        /// Full domain name
        /// </summary>
        private string domainName;

        /// <summary>
        /// Short domain name
        /// </summary>
        private string shortDomain;

        /// <summary>
        /// Lock object for multithreading search
        /// </summary>
        private object lockObj = new object();

        /// <summary>
        /// links which are already proceded
        /// </summary>
        private HashSet<string> links = new HashSet<string>();

        /// <summary>
        /// found products
        /// </summary>
        private HashSet<Product> products;

        /// <summary>
        /// Object which load documents
        /// </summary>
        private HtmlWeb web = new HtmlWeb();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainName">full domain name like http://domain.com/</param>
        public ShopWrapper(string domainName)
        {
            this.domainName = domainName;
            this.shortDomain = GetDomain.GetDomainFromUrl(domainName);
            links.Add(domainName);
        }

        /// <summary>
        /// Search for all products in the shop
        /// </summary>
        /// <returns></returns>
        public HashSet<Product> WrapShop()
        {
            products = new HashSet<Product>();
            Wrap(domainName);
            return products;
        }

        /// <summary>
        /// Wrap a single page and recursively go tho all incoming links
        /// </summary>
        /// <param name="url"></param>
        private void Wrap(string url)
        {
            if (products.Count >= MaxProducts) return;
            Debug.WriteLine("parsing page:" + url);
            var htmlDoc = web.Load(url);

            foreach (var prod in htmlDoc.GetProducts(domainName, shortDomain))
            {
                if (AddIfNotEqual(products, prod))
                {
                    if (products.Count >= MaxProducts) return;
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

                //TODO: Change this code to:
                //Task t= new Task(()=>Wrap(link));
                //t.Start();
                //tasks.Add(t);
                //change collections to asynchronous
                Wrap(link);

                if (products.Count >= MaxProducts) return;

            }

            //TODO: add Task.WaitAll(tasks); 
        }

        //TODO: add to Product class IComparable interface and remove this method
        /// <summary>
        /// Add product to the products if products don't contain product
        /// </summary>
        /// <param name="products"></param>
        /// <param name="product"></param>
        /// <returns></returns>
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