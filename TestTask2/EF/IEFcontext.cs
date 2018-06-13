using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using TestTask2.Models;

namespace TestTask2.EF
{
    public interface IEFContext : IDisposable
    {
        DbSet<Product> Products { get; set; }
        DbSet<Image> Images { get; set; }
        int SaveChanges();
        void LoadImages(Product p);
    }
}