using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace TestTask2.StringUtils
{
    public static class CurrencyRegex
    {
        private static string priceStr = @"[\d\s.,'`]*[\d]+[\d\s.,'`]*";
        private static string connectionStr = @"\s*";

        public static string[] currencyTrims = { ".", ",", "'", "`", " ", "\t" };

        public static Regex price = new Regex(priceStr, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static List<Regex> regices = new List<Regex>()
        {
            new Regex(priceStr+connectionStr+@"грн",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant),
            new Regex(priceStr+connectionStr+@"руб",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant),
            new Regex(@"\s\$"+connectionStr+priceStr,RegexOptions.IgnoreCase|RegexOptions.CultureInvariant),
        };

        public static string CutPrice(string s)
        {
            foreach(string c in currencyTrims)
            {
                s = s.Replace(c, String.Empty);
            }

            return s;
        }

    }
}