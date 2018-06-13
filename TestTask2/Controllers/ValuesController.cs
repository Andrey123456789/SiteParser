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

        [HttpGet]
        [Route("api/values/getproducts/")]
        public IEnumerable<Product> GetProducts(string domain = "")
        {
            if (Uri.TryCreate(domain, UriKind.Absolute, out Uri uri))
            {
                var shopWrapper = new ShopWrapper(uri.AbsoluteUri);

                var products = shopWrapper.WrapShop();

                foreach (var product in products)
                {
                    //adding new images to database:

                    foreach (var image in product.Images)
                    {
                        db.Images.Add(image);
                    }

                    var prds = db.Products.Where(x => x.Description == product.Description && x.Domain == product.Domain);

                    if (prds.Count() > 1) throw new Exception("Database allows product with equal names for one domain");

                    if (prds.Count() == 1)
                    {
                        var p = prds.First();
                        db.LoadImages(product);

                        List<Image> oldImages = p.Images.ToList();

                        oldImages.ForEach(img => p.Images.Remove(img));

                        foreach(var img in product.Images)
                        {
                            p.Images.Add(img);
                        }

                        p.DeltaPrice = p.Price - product.Price;
                        p.Price = product.Price;

                        db.Products.Attach(p);
                    }
                    else
                    {
                        db.Products.Add(product);
                    }
                }
                db.SaveChanges();

            }

            return db.Products.ToList();

        }

        [HttpGet]
        public ProductExt GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            db.LoadImages(product);
            HashSet<string> imagesBase64 = new HashSet<string>();
            foreach(var img in product.Images)
            {
                imagesBase64.Add(Convert.ToBase64String(img.Picture));
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
