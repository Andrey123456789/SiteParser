using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestTask2.Models
{
    public abstract class AProduct : ModelId
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [DefaultValue(0)]
        public decimal DeltaPrice { get; set; }

        [Required]
        public string Domain { get; set; }

        public virtual Currency Currency { get; set; }

        public AProduct(string domain, string descrption, int price, Currency currency)
        {
            this.Description = descrption;
            this.Price = price;
            this.Domain = domain;
            this.Currency = currency;
        }

        public AProduct()
        {

        }
    }
}