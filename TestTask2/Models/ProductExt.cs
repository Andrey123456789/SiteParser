using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestTask2.Models
{
    /// <summary>
    ///  Class used to send product with images coded in base64
    /// </summary>
    public class ProductExt:AProduct
    {
        public virtual IEnumerable<string> ImagesBase64 { get; set; }

        public ProductExt(AProduct p, IEnumerable<string> imagesBase64) : base()
        {
            this.Id = p.Id;
            this.Domain = p.Domain;
            this.Description = p.Description;
            this.DeltaPrice = p.DeltaPrice;
            this.Price = p.Price;
            this.ImagesBase64 = imagesBase64;
        }
    }
}