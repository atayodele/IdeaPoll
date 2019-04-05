using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdeaSolution.Services.Dto
{
    public class UserForUpdateDto
    {
        public string Firstname { get; set; }
        public string Othername { get; set; }
        public string PhoneNumber { get; set; } 
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
