﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdeaSolution.Services.Dto
{
    public class IdeaCreationDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string FilePath { get; set; }
        public IFormFile File { get; set; }
        public IdeaCreationDto()
        {
            DateAdded = DateTime.Now;
        }
    }
}
