using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TestTask2.StringUtils;

namespace TestTask2.EF
{
    public class DBInit: CreateDatabaseIfNotExists<EFContext>
    {
        protected override void Seed(EFContext context)
        {
            foreach (var c in Currencies.CurrencyValues)
            {
                context.Currencies.Add(c);
            }
            context.SaveChanges();
        }
    }
}