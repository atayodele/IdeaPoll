﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdeaSolution.Services.Dto
{
    public class RegisterForDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify a password btw 4 and 8 characters")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Othername { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; }

        public string[] Roles { get; set; }
        [Required]
        public string UserName => $"{Email}";
        //public string Fullname => $"{Firstname} {Othername}".Trim();


        public RegisterForDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}
