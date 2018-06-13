using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace TestTask2.StringUtils
{
    public class CurrencyRegex:Regex
    {
        public string CurrencyStr { get; }

        public char CurrencySign { get; }

        public CurrencyRegex(string pattern,char currencySign, string currencyStr) : base(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
        {
            CurrencySign = currencySign;
            CurrencyStr = currencyStr;
        }
    }
}