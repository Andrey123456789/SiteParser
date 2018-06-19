using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TestTask2.Models;

namespace TestTask2.StringUtils
{
    public class Currencies
    {

        private static string priceStr = @"[\d\s\n_separators_]*[\d]+_decimalSeparator_[\d\s\n_separators_]*";//@"[\d\s\.,'`]*[\d]+[\d\s\.,'`]*";

        private static string connectionStr = @"\s*";

        private static string ending = @"($|[\W])";

        /// <summary>
        /// modified pattern priceStr with substituted _separators_ and _decimalSeparator_
        /// </summary>
        private string priceStrM;

        // public static string[] currencyTrims = { ".", ",", "'", "`", " ", "\t" };

        //public static Regex price = new Regex(priceStr, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static List<Currency> CurrencyValues => new List<Currency>(){
            new Currency("Hrivna","UAH",'₴',"грн"),

            new Currency("Dollar","USD",'$',"dol"),

            new Currency("Euro","EUR",'€', "eur"),

            new Currency("Rubble","RUB",'₽',"руб"),

            };

        public Currencies(string decimalSeparator, string[] currencySeparators)
        {
            if (decimalSeparator != "")
            {
                decimalSeparator = "[" + decimalSeparator + "]?";
            }
            priceStrM = priceStr.Replace("_decimalSeparator_", decimalSeparator);
            string separatorsStr = String.Join("", currencySeparators);
            priceStrM = priceStrM.Replace("_separators_", separatorsStr);
        }

        /// <summary>
        /// Gets all currency regexes
        /// </summary>
        /// <returns></returns>
        public List<CurrencyRegexes> GetAllCurRegexes()
        {
            var curRegexes = new List<CurrencyRegexes>
            {
                new CurrencyRegexes(CurrencyValues[0], new List<Regex>(){
                BuildCurrencyRegex(@"грн"),
                BuildCurrencyRegex(@"UAH"),
            }),

                new CurrencyRegexes(CurrencyValues[1], new List<Regex>(){
                BuildCurrencyInvertedRegex(@"\$"),
                BuildCurrencyRegex(@"USD"),
            }),

                new CurrencyRegexes(CurrencyValues[2], new List<Regex>(){
                BuildCurrencyRegex(@"€"),
                BuildCurrencyRegex(@"EUR"),
            }),

                new CurrencyRegexes(CurrencyValues[3], new List<Regex>(){
                BuildCurrencyRegex(@"₽"),
                BuildCurrencyRegex(@"руб"),
                BuildCurrencyRegex(@"RUB"),
            })
            };

            return curRegexes;
        }

        /// <summary>
        /// Gets regexs, which searhces a price value
        /// </summary>
        /// <returns></returns>
        public Regex GetPriceRegex()
        {
            return new Regex(priceStrM, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        /// <summary>
        /// Build regex for sring like "some text.... 100'000 грн some text..."
        /// </summary>
        /// <param name="currencyPattern">pattern determining currency string (like грн)</param>
        /// <returns></returns>
        private Regex BuildCurrencyRegex(string currencyPattern)
        {

            return new Regex(priceStrM + connectionStr + currencyPattern + ending, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        /// <summary>
        /// Build regex for sring like "some text.... $100'000 some text..."
        /// </summary>
        /// <param name="currencyPattern">pattern determining currency string (like $)</param>
        /// <returns></returns>
        private Regex BuildCurrencyInvertedRegex(string currencyPattern)
        {
            return new Regex(currencyPattern + connectionStr + priceStrM, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        /// <summary>
        /// Remove non-numeric characters from string, containing price number
        /// </summary>
        /// <param name="s">price string</param>
        /// <param name="strsToRemove">string patterns to remove from price string</param>
        /// <returns></returns>
        public static string CutPrice(string s, string[] strsToRemove)
        {
            foreach (string c in strsToRemove)
            {
                s = s.Replace(c, String.Empty);
            }

            return s;
        }
    }
}