using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestTask2.StringUtils;

namespace TestTask2.EF
{
    public static class DBInit
    {
        public static void InitCurrencies()
        {
            using (EFContext context = new EFContext())
            {
                if (context.Currencies.Count() == 0)
                {
                   foreach(var c in Currencies.CurrencyValues)
                    {
                        context.Currencies.Add(c);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}