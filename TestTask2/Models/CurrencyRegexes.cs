using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace TestTask2.Models
{
    public class CurrencyRegexes:Currency
    {
        [NotMapped]
        public List<Regex> Regexes { get; }

        public CurrencyRegexes(Currency cur, List<Regex> regexes) : base(cur.Name, cur.Code, cur.Symbol, cur.Short)
        {
            this.Regexes = regexes;
        }

        public CurrencyRegexes()
        {

        }
    }
}