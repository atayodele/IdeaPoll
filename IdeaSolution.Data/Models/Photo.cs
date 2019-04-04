using System;
using System.Collections.Generic;
using System.Text;

namespace IdeaSolution.Data.Models
{
    public class Photo
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }

        public AppUser User { get; set; }
        public string UserId { get; set; }
    }
}
