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
        public int Price { get; set; }

        [DefaultValue(0)]
        public int DeltaPrice { get; set; }

        [Required]
        public string Domain { get; set; }

        public AProduct(string domain, string descrption, int price)
        {
            this.Description = descrption;
            this.Price = price;
            this.Domain = domain;
        }

        public AProduct()
        {

        }
    }
}