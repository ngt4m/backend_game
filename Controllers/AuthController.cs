using System.Linq;
using System.Web.Http;
using System;
using TVQD_API.Models.DTOs;
using TVQD_API.Help;
using System.Web;
using System.Net.Http;
using System.Net;


namespace TVQD_API.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly AppDbContext _db = new AppDbContext();

        [HttpPost]
        [Route("loginorregister")]
        public IHttpActionResult LoginOrRegister(UserDTO model)
        {

            if (model == null)
                return BadRequest("User data is required.");

            //Kiểm tra thông tin người chơi có hợp lệ không
            if (string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrWhiteSpace(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest( message: "Thông tin không được để trống!");
            }

            //Kiểm tra tên hợp lệ
            if(ProfanityHelper.ContainsProfanity(model.FullName) || ProfanityHelper.ContainsProfanity(model.Email))
            {
                return BadRequest("Thông tin không hợp lệ");
            }

            var user = _db.Users.FirstOrDefault(u => u.PhoneNumber == model.PhoneNumber);

            string userIp = HttpContext.Current?.Request?.UserHostAddress;


            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    CreateDate = DateTime.Now,
                    Status = 1
                };
                _db.Users.Add(user);
            }

            user.FullName = model.FullName;
            user.LastLoginTime = DateTime.Now;
            user.IpAddress = userIp;


            _db.SaveChanges();


            var token = JwtHelper.GenerateToken(user.Id, user.PhoneNumber);
            return Ok(new
            {
                Token = token,
                UserId =  user.Id
            });
        }
    }
}