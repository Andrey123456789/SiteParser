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

        static EFContext()
        {
            Database.SetInitializer(new DBInit());
        }

        public EFContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        public void LoadConnections(Product p)
        {
            Entry(p).Collection(x => x.Images).Load();
            Entry(p).Reference(x => x.Currency).Load();
        }

        public void LoadCurrency(Product p)
        {
            Entry(p).Reference(x => x.Currency).Load();
        }

        public Currency FindCurrency(string code)
        {
            return Currencies.Where(c => c.Code == code).FirstOrDefault();
        }

    }
}