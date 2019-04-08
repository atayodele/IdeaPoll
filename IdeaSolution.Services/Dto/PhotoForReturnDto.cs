using System;
using System.Collections.Generic;
using System.Text;

namespace IdeaSolution.Services.Dto
{
    public class PhotoForReturnDto
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
    }
}
