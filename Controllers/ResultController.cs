using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TVQD_API.Help;
using TVQD_API.Models.DTOs;

namespace TVQD_API.Controllers
{
    [RoutePrefix("api/result")]
    public class ResultController : ApiController
    {
        private readonly AppDbContext _db = new AppDbContext();

        [Authorize]
        [HttpPost]
        [Route("submit")]
        public IHttpActionResult SubmitResult(UserResultDTO model)
        {
            var user = _db.Users.FirstOrDefault(c => c.Id == model.UserId);
            if(user == null)
                return BadRequest("Người dùng không hợp lệ.");


            var contest = _db.Contests.FirstOrDefault(c => c.Id == model.ContestId);
            if (contest == null || contest.AntiCheatToken != model.AntiCheatToken)
                return BadRequest("AntiCheatToken không hợp lệ.");

            string token = TokenHelper.GenerateSubmissionToken(model.UserId, model.ContestId, model.StartTime);

            bool tokenExists = _db.UserResults.Any(x => x.SubmissionToken == token);
            if (tokenExists) return BadRequest("Kết quả này đã được gửi.");


            var result = new UserResult
            {
                Id = Guid.NewGuid().ToString(),
                UserId = model.UserId,
                ContestId = model.ContestId,
                NumberCorrect = model.NumberCorrect,
                TotalTime = model.TotalTime,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                DeviceInfo = model.DeviceInfo,
                IpAddress = HttpContext.Current?.Request?.UserHostAddress,
                SubmissionToken = token
            };

            _db.UserResults.Add(result);
            _db.SaveChanges();

            return Ok("Đã lưu kết quả.");
        }

        [Authorize]
        [HttpGet]
        [Route("top/{contestId}")]
        public IHttpActionResult GetTop10Results(string contestId)
        {
            var contest = _db.Contests.FirstOrDefault(c => c.Id == contestId);
            if (contest == null)
                return NotFound();

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

            return Ok(result);
        }


        [Authorize]
        [HttpGet]
        [Route("top-total-4weeks")]
        public IHttpActionResult GetTop4WeeksAggregate()
        {
            // 1. Lấy 4 contest mới nhất
            var recentContests = _db.Contests
                .OrderByDescending(c => c.StartDate)
                .Take(4)
                .Select(c => c.Id)
                .ToList();

            // 2. Lấy kết quả tốt nhất của mỗi người trong mỗi contest
            var bestResults = _db.UserResults
                .Where(r => recentContests.Contains(r.ContestId))
                .GroupBy(r => new { r.UserId, r.ContestId })
                .Select(g => g.OrderByDescending(r => r.NumberCorrect)
                              .ThenBy(r => r.TotalTime)
                              .FirstOrDefault())
                .ToList();

            // 3. Tổng hợp lại theo người chơi
            var userAggregates = bestResults
                .GroupBy(r => r.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalCorrect = g.Sum(x => x.NumberCorrect),
                    TotalTime = g.Sum(x => x.TotalTime),
                    ContestCount = g.Count()
                })
                .Where(x => x.ContestCount == 4) // chỉ người đã thi cả 4 tuần
                .ToList();

            // 4. Join với Users để lấy thông tin
            var topAggregates = userAggregates
                .Join(_db.Users,
                      agg => agg.UserId,
                      user => user.Id,
                      (agg, user) => new
                      {
                          user.FullName,
                          user.PhoneNumber,
                          user.Email,
                          agg.TotalCorrect,
                          agg.TotalTime
                      })
                .OrderByDescending(x => x.TotalCorrect)
                .ThenBy(x => x.TotalTime)
                .Take(10)
                .ToList();

            return Ok(topAggregates);
        }

    }

}