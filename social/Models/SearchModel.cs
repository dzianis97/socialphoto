using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace social.Models
{
    public class SearchModel
    {
        public String UserName { get; set; }
        public String SurName { get; set; }
        public DateTime AgeMin { get; set; }
        public DateTime AgeMax { get; set; }
        public string Gender { get; set; }
    }
}