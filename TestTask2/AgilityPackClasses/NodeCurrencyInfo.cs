using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TestTask2.Models;

namespace TestTask2.AgilityPackClasses
{
    public class NodeCurrencyInfo
    {
        public HtmlNode Node { get; }

        public Currency Currency { get; }

        public Regex CurrencyRegex { get; }

        public NodeCurrencyInfo(HtmlNode node, Currency currency, Regex currencyRegex)
        {
            this.Node = node;
            this.Currency = currency;
            this.CurrencyRegex = currencyRegex;
        }
    }
}