using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestTask2.Models
{
    public class Image : ModelId
    {

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