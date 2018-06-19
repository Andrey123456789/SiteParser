using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TestTask2.AgilityPackClasses;
using TestTask2.StringUtils;

namespace TestTask2.Models
{
    public class ParseDomainParams
    {
        /// <summary>
        /// Full domain name, ex. "http://domain.com"
        /// </summary>
        public string Domain { get; }

        /// <summary>
        /// Short domain name, ex. "domain.com"
        /// </summary>
        public string ShortDomain { get; }

        /// <summary>
        /// Set of strings to replace in product.Description string
        /// </summary>
        public IEnumerable<string> ReplaceInDecription { get; }

        /// <summary>
        /// Set of strings to skip product which contains one of the strings in its description
        /// </summary>
        public IEnumerable<string> SkipDecriptionWhenFound { get; }

        /// <summary>
        /// Separators between groups of degrees
        /// </summary>
        public string[] CurrencySeparators { get; }

        /// <summary>
        /// Separator between integer and fractional parts
        /// </summary>
        public string DecimalSeparator { get; }

        /// <summary>
        /// A Kind of getting a product.Description
        /// </summary>
        public DescriptionGetKind DescriptionGetKind { get; set; }

        public SearchPriceKind SearchPriceKind { get; set; }

        /// <summary>
        /// Currency Regexes to find price value
        /// </summary>
        public List<CurrencyRegexes> CurRegexes { get;}

        public Regex PriceRegex { get; }


        public ParseDomainParams(string Domain, IEnumerable<string> ReplaceInDecription, IEnumerable<string> SkipDecriptionWhenFound,
            string[] CurrencySeparators, string decimalSeparator, DescriptionGetKind descriptionGetKind, SearchPriceKind searchPriceKind)
        {
            this.Domain = Domain;
            ShortDomain = GetDomain.GetDomainFromUrl(Domain);
            this.ReplaceInDecription = ReplaceInDecription;
            this.SkipDecriptionWhenFound = SkipDecriptionWhenFound;
            this.CurrencySeparators = CurrencySeparators;
            this.DecimalSeparator = decimalSeparator;
            this.DescriptionGetKind = descriptionGetKind;
            this.SearchPriceKind = searchPriceKind;

            var currencies = new Currencies(DecimalSeparator, CurrencySeparators);

            CurRegexes = currencies.GetAllCurRegexes();
            PriceRegex = currencies.GetPriceRegex();
        }

        ParseDomainParams()
        {

        }


    }
}