using System;
using System.Collections.Generic;
using System.Text;

namespace IdeaSolution.Data.Models
{
    public class Idea
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsRead { get; set; }
        public string FilePath { get; set; }

        public AppUser User { get; set; }
        public string UserId { get; set; }
    }
}
