using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace TestTask2.Models
{
    public class Currency:ModelId
    {
        /// <summary>
        /// Currency name (Hrivna etc.)
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Code of the currency, like UAH, USD etc.
        /// </summary>
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// Symbol of the currency
        /// </summary>
        [Required]
        public string Symbol { get; set; }

        /// <summary>
        /// Short value of the currency, like грн
        /// </summary>
        [Required]
        public string Short { get; set; }

        public Currency(string name, string code, char symbol, string @short)
        {
            this.Name = name;
            this.Code = code;
            this.Symbol = symbol.ToString();
            this.Short = @short;
        }

        public Currency()
        {

        }

    }
}