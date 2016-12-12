using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace social.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public string UserId { get; set; }
        public virtual User UserLiked { get; set; }
        public virtual Photo PhotoLiked { get; set; }
    }
}