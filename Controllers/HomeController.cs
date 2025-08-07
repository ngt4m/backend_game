using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TVQD_API.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db = new AppDbContext();
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string taikhoan, string matkhau)
        {
            if (taikhoan != "AdminTVQD" || matkhau != "Tvqd15111957")
            {
                ViewBag.Error = "Thông tin đăng nhập không chính xác";
                return View();
            } 
                


            return Redirect("TopUser");
        }

        public ActionResult TopUser(int contestIndex = 1)
        {

            var contest =  _db.Contests
            .OrderByDescending(c => c.StartDate)
            .Skip(contestIndex - 1)
            .FirstOrDefault();

            if (contest == null)
                return Redirect("/");

            string contestId = contest.Id;

            // Lấy kết quả tốt nhất của từng người trong contest này
            var bestPerUser = _db.UserResults
                .Where(r => r.ContestId == contestId)
                .GroupBy(r => r.UserId)
                .Select(g => g
                    .OrderByDescending(r => r.NumberCorrect)
                    .ThenBy(r => r.TotalTime)
                    .FirstOrDefault())
                .ToList();

            // Sắp xếp toàn bộ người chơi theo thứ hạng rồi lấy top 10
            var top10 = bestPerUser
                .OrderByDescending(r => r.NumberCorrect)
                .ThenBy(r => r.TotalTime)
                .Take(10)
                .ToList();

            // Kèm thông tin người dùng
            var result = top10
                .Join(_db.Users,
                      r => r.UserId,
                      u => u.Id,
                      (r, u) => new
                      {
                          u.FullName,
                          u.PhoneNumber,
                          u.Email,
                          r.NumberCorrect,
                          r.TotalTime,
                          r.StartTime,
                          r.EndTime
                      })
                .ToList();


            return View(result);
        }
    }



}
