using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TestTask2.Extensions;
using TestTask2.Models;
using TestTask2.StringUtils;

namespace TestTask2.AgilityPackClasses
{
    /// <summary>
    /// A kind of getting product.Description
    /// </summary>
    public enum DescriptionGetKind
    {
        /// <summary>
        /// A full text of the found segment(-price value) is taken as a Description
        /// </summary>
        dgkFull=0,

        /// <summary>
        /// The longest text of the sub-element of the found segment(-price value) is taken as a Description
        /// </summary>
        dgkLongest=1
    }

    public enum SearchPriceKind
    {
        /// <summary>
        /// Searches maximum outer node, containing single price value
        /// </summary>
        spkOuter=0,

        /// <summary>
        /// Searches inner node, containing single price value, then goes up, until finds one or more images
        /// </summary>
        spkInner=1
    }

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
        /// Lock object for multithreading search
        /// </summary>
        private object lockObj = new object();

        /// <summary>
        /// Links which are already proceded
        /// </summary>
        private HashSet<string> links = new HashSet<string>();

        /// <summary>
        /// Found products
        /// </summary>
        private HashSet<Product> products;

        /// <summary>
        /// Object which loads documents
        /// </summary>
        private HtmlWeb web = new HtmlWeb();

        private ParseDomainParams pdp;
      
        public ShopWrapper(ParseDomainParams pdp)
        {
            this.pdp = pdp;

            //this.shortDomain = GetDomain.GetDomainFromUrl(domainName);
            links.Add(pdp.Domain);
        }

        /// <summary>
        /// Search for all products in the shop
        /// </summary>
        /// <returns></returns>
        public HashSet<Product> WrapShop()
        {
            products = new HashSet<Product>();
            Wrap(pdp.Domain);
            return products;
        }

        //TODO: Add parsing using schema.org
        /// <summary>
        /// Wrap a single page and recursively go to all incoming links
        /// </summary>
        /// <param name="url"></param>
        private void Wrap(string url)
        {
            if (products.Count >= MaxProducts) return;
            Debug.WriteLine("parsing page:" + url);
            var htmlDoc = web.Load(url);
            foreach (var prod in htmlDoc.GetProducts(pdp))
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
                    domain = pdp.ShortDomain;
                    link = pdp.Domain + link;
                }
                else
                {
                    domain = GetDomain.GetDomainFromUrl(uri);
                    if (domain != pdp.ShortDomain) continue;
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

        //TODO: add to Product interface IComparable and remove this method
        /// <summary>
        /// Add product to the products if products don't contain product
        /// </summary>
        /// <param name="products"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private static bool AddIfNotEqual(HashSet<Product> products, Product product)
        {
            if (!products.Any(x => x.Description == product.Description && x.Price == product.Price && x.Currency.Code==product.Currency.Code))
            {
                products.Add(product);
                return true;
            }

            return false;
        }

    }
}