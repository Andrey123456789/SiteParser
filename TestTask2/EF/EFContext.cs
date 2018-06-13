using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using TestTask2.Models;

namespace TestTask2.EF
{
    public class EFContext : DbContext, IEFContext
    {
        public EFContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }

        public void LoadImages(Product p)
        {
            Entry(p).Collection(x => x.Images).Load();
        }
    }
}