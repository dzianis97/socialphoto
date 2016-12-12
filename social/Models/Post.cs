using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace social.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string PublishedTime { get; set; }
        public string Image { get; set; }
        public string SenderId { get; set; }
        public int LikesCount { get; set; }
        public string SenderName { get; set; }
        public string SenderPhoto { get; set; }
    }
}