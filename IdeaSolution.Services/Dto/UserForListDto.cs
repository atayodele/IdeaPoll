using System;
using System.Collections.Generic;
using System.Text;

namespace IdeaSolution.Services.Dto
{
    public class UserForListDto
    {
        public string Email { get; set; }       
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Othername { get; set; }
        public string PhoneNumber { get; set; }
        public string[] Roles { get; set; }
        public string UserName { get; set; }
        public string Fullname => $"{UserName} {Othername}".Trim();
    }
}
