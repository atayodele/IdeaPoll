using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdeaSolution.Services.Dto
{
    public class RoleForDto
    {
        [Required]
        public string Name { get; set; }
    }
}
