using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace social.Models
{
    public class Photo
    {


        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedTime { get; set; }
        public string Image { get; set; }
        public string SenderId { get; set; }
        public int LikesCount { get; set; }
        public bool isPrivate { get; set; }
        public virtual User Sender { get; set; }
        


    }
}