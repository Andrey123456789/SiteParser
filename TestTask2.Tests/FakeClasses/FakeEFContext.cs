using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask2.EF;
using TestTask2.Models;

namespace TestTask2.Tests.FakeClasses
{
    class FakeEFContext : IEFContext
    {
        public System.Data.Entity.DbSet<Product> Products { get; set ; }
        public System.Data.Entity.DbSet<Image> Images { get; set; }

        public void Dispose()
        {

        }

        public void LoadImages(Product p)
        {
            
        }

        public int SaveChanges()
        {
            return 0;
        }

        public FakeEFContext():base()
        {
            Images = new TestDbSet<Image>();
            Products = new TestDbSet<Product>();
            byte[] picture = { 1, 2, 3, 4 };

          for(int i=1; i<10; i++)
            {
                var image = new Image(picture) { Id = i };
                Images.Add(image);
                Products.Add(new Product("www.mydomain.com", i.ToString(), i, new HashSet<Image>() { image }) { Id = i });
            }
        }
    }
}
