using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace social.Models
{
    public class UploadPhotoModel
    {


        [Required]
            public string Title { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        [Required]
        public HttpPostedFileBase Image { get; set; }
        
        [Required]
        public bool isPrivate { get; set; }
       



     
}
}