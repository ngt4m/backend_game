using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TVQD_API.Models.DTOs
{
    // DTO khi đăng nhập
    public class UserDTO
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}