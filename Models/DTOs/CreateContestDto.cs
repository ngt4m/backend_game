using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TVQD_API.Models.DTOs
{
    public class CreateContestDto
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxAttempts { get; set; } = 1;
    }

}