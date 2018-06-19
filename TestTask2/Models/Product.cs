using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestTask2.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class Product : AProduct
    {

        public virtual ICollection<Image> Images { get; set; }

        public Product(string domain, string descrption, decimal price, ICollection<Image> images, Currency currency) : base(domain, descrption, price, currency)
        {
            this.Images = images;
        }

        public Product() : base()
        {

        }
    }
}