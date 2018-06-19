using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestTask2.Models
{
    public class Image : ModelId
    {
        /// <summary>
        /// Byte representation of picture
        /// </summary>
        public byte[] Picture { get; set; }

        /// <summary>
        /// Extension of the image(jpg, png)
        /// </summary>
        public string Extension { get; set; }

        public Image(byte[] picture, string extension)
        {
            this.Picture = picture;
            this.Extension = extension;
        }

        public Image()
        {

        }
    }
}