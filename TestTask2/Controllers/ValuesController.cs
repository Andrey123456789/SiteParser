using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestTask2.AgilityPackClasses;
using TestTask2.EF;
using TestTask2.Models;

namespace TestTask2.Controllers
{
    public class ValuesController : ApiController
    {

        private IEFContext db = new EFContext();
        [HttpPost]
        [HttpGet]
        [Route("api/values/getproducts/")]
        public IEnumerable<Product> GetProducts([FromBody] ParseDomainParams pdp)
        {
            if (pdp != null)
            {
                System.Diagnostics.Debug.WriteLine("Parse enums:" + pdp.DescriptionGetKind + " " + pdp.SearchPriceKind);
            }
            //var pdp1 = new ParseDomainParams("https://www.olx.ua", new HashSet<string>(), new HashSet<string>(), new string[] { "" }, "", 0, 0);
            if (pdp != null && Uri.TryCreate(pdp.Domain, UriKind.Absolute, out Uri uri))
            {
                var shopWrapper = new ShopWrapper(pdp);

                var products = shopWrapper.WrapShop();

                foreach (var product in products)
                {
                    //adding new images to database:

                    foreach (var image in product.Images)
                    {
                        db.Images.Add(image);
                    }

                    //TODO: Add try catch exception
                    var p = db.Products.Any() ? db.Products.Where(x => x.Description == product.Description && x.Domain == product.Domain && x.Currency.Code == product.Currency.Code).SingleOrDefault() : null;

                    if (p != null)
                    {
                        db.LoadConnections(p);

                        List<Image> oldImages = p.Images.ToList();

                        oldImages.ForEach(img => p.Images.Remove(img));

                        foreach (var img in product.Images)
                        {
                            p.Images.Add(img);
                        }

                        p.DeltaPrice = p.Price - product.Price;
                        p.Price = product.Price;
                        p.Currency = db.FindCurrency(p.Currency.Code);
                        db.Products.Attach(p);
                    }
                    else
                    {
                        product.Currency = db.FindCurrency(product.Currency.Code);
                        db.Products.Add(product);
                    }
                }
                db.SaveChanges();

            }
            var list = db.Products.ToList();
            list.ForEach(x => db.LoadCurrency(x));
            return list;

        }

        [HttpGet]
        public ProductExt GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            db.LoadConnections(product);
            HashSet<string> imagesBase64 = new HashSet<string>();
            foreach (var img in product.Images)
            {
                imagesBase64.Add(img.Extension + "; base64," + Convert.ToBase64String(img.Picture));
            }
            return new ProductExt(product, imagesBase64);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ValuesController()
        {

        }

        public ValuesController(IEFContext context)
        {
            db = context;
        }
    }
}
