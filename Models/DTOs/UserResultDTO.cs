using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TVQD_API.Models.DTOs
{
    public class UserResultDTO
    {
        public string UserId { get; set; }
        public string ContestId { get; set; }
        public int NumberCorrect { get; set; }
        public double TotalTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string DeviceInfo { get; set; }
        public string IpAddress { get; set; }
        public string AntiCheatToken { get; set; }
    }


}