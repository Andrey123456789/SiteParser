using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace TestTask2.StringUtils
{
    public static class StringConstants
    {
        public static HashSet<Regex> linksToSkip = new HashSet<Regex>()
        {
           new Regex(@"^mailto:",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant),
           new Regex(@"^javascript:",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant),
        };
    }
}