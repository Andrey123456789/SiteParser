using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestTask2.Models
{
    public class Image
    {
        public int? Id { get; set; }
        public byte[] Picture { get; set; }

        public Image(byte[] picture)
        {
            this.Picture = picture;
        }

        public Image()
        {

        }
    }
}