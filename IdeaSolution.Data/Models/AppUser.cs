using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IdeaSolution.Data.Models
{
    public class AppUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Othername { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public ICollection<Photo> Photos { get; set; }
        public ICollection<Idea> Ideas { get; set; }
        public AppUser()
        {
            Photos = new Collection<Photo>();
        }
    }
}
