using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace social.Models
{
    public class LogModel
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public string ActionName { get; set; }
        public DateTime DateTime { get; set; }
        public string Content { get; set; }
    }
}