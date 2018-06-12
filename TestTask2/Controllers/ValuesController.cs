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

        EFContext db = new EFContext();

        [HttpGet]
        [Route("api/values/getproducts/")]
        public IEnumerable<Product> GetProducts(string domain = "")
        {
            if (Uri.TryCreate(domain, UriKind.Absolute, out Uri uri))
            {
                var shopWrapper = new ShopWrapper(domain);

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
                        db.Entry(p).Collection(x => x.Images).Load();

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
            db.Entry(product).Collection(x => x.Images).Load();
            HashSet<string> imagesBase64 = new HashSet<string>();
            foreach(var img in product.Images)
            {
                imagesBase64.Add(Convert.ToBase64String(img.Picture));
            }
            return new ProductExt(product, imagesBase64);
        }

        //[HttpPost]
        //public void CreateBook([FromBody]Book book)
        //{
        //    db.Books.Add(book);
        //    db.SaveChanges();
        //}

        //[HttpPut]
        //public void EditBook(int id, [FromBody]Product product)
        //{
        //    if (id != product.Id) return;

        //    db.Products.Attach(product);


        //    //var authors = book.Authors;
        //    //var genres = book.Genres;

        //    //book.Authors = null;
        //    //book.Genres = null;

        //    //db.Books.Attach(book);
        //    //db.Entry(book).State = EntityState.Modified;

        //    //var existingBook = db.Books.Find(book.Id);

        //    //db.Entry(existingBook).Collection(i => i.Authors).Load();
        //    //db.Entry(existingBook).Collection(i => i.Genres).Load();
        //    //var deletedAuthors = existingBook.Authors.Where(oldA => !authors.Any(newA => newA.Id == oldA.Id)).ToList();
        //    //var addedAuthors = authors.Where(newA => !existingBook.Authors.Any(oldA => oldA.Id == newA.Id)).ToList();

        //    //deletedAuthors.ForEach(a => existingBook.Authors.Remove(a));

        //    //foreach (var a in addedAuthors)
        //    //{
        //    //    if (db.Entry(a).State == EntityState.Detached)
        //    //    {
        //    //        if (a.Id != null)
        //    //            db.Authors.Attach(a);
        //    //        else
        //    //            db.Authors.Add(a);
        //    //    }
        //    //    existingBook.Authors.Add(a);
        //    //}

        //    //var deletedGenres = existingBook.Genres.Where(oldG => !genres.Any(newG => newG.Id == oldG.Id)).ToList();
        //    //var addedGenres = genres.Where(newG => !existingBook.Genres.Any(oldG => oldG.Id == newG.Id)).ToList();

        //    //deletedGenres.ForEach(g => existingBook.Genres.Remove(g));

        //    //foreach (var g in addedGenres)
        //    //{
        //    //    if (db.Entry(g).State == EntityState.Detached)
        //    //    {
        //    //        if (g.Id != null)
        //    //            db.Genres.Attach(g);
        //    //        else
        //    //            db.Genres.Add(g);
        //    //    }

        //    //    existingBook.Genres.Add(g);
        //    //}

        //    ////existingBook.Name = book.Name;
        //    ////existingBook.Publisher = book.Publisher;
        //    ////existingBook.Year = book.Year;

        //    //db.SaveChanges();

        //}

        //[HttpDelete]
        //public void DeleteBook(int id)
        //{
        //    Book book = db.Books.Find(id);
        //    if (book != null)
        //    {
        //        db.Books.Remove(book);
        //        db.SaveChanges();
        //    }
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        static ValuesController()
        {

        }
    }
}
