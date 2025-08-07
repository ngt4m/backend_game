using System.Linq;
using System.Web.Http;
using TVQD_API.Models;

namespace TVQD_API.Controllers
{
    [RoutePrefix("api/stats")]
    public class StatsController : ApiController
    {
        private readonly AppDbContext _db = new AppDbContext();

        [Authorize]
        [HttpGet]
        [Route("contest-summary")]
        public IHttpActionResult GetContestSummary(string contestId)
        {
            var results = _db.UserResults.Where(r => r.ContestId == contestId).ToList();
            if (!results.Any()) return NotFound();

            var stats = new
            {
                TotalUsers = results.Count,
                AverageCorrect = results.Average(r => r.NumberCorrect),
                AverageTime = results.Average(r => r.TotalTime),
                MaxCorrect = results.Max(r => r.NumberCorrect),
                MinCorrect = results.Min(r => r.NumberCorrect),
                LastSubmission = results.Max(r => r.EndTime)
            };

            return Ok(stats);
        }

        [Authorize]
        [HttpGet]
        [Route("users-in-contest")]
        public IHttpActionResult GetUsersInContest(string contestId)
        {
            var users = _db.UserResults
                .Where(r => r.ContestId == contestId)
                .Select(r => new
                {
                    r.User.FullName,
                    r.User.PhoneNumber,
                    r.NumberCorrect,
                    r.TotalTime,
                    r.EndTime
                }).ToList();

            return Ok(users);
        }

        [Authorize]
        [HttpGet]
        [Route("user-result")]
        public IHttpActionResult GetUserResult(string userId, string contestId)
        {
            var result = _db.UserResults
                .Where(r => r.UserId == userId && r.ContestId == contestId)
                .OrderByDescending(r => r.NumberCorrect)   // Ưu tiên điểm cao
                .ThenBy(r => r.TotalTime)                  // Nếu trùng điểm, chọn thời gian thấp
                .FirstOrDefault();

            if (result == null) return NotFound();

            return Ok(new
            {
                result.NumberCorrect,
                result.TotalTime,
                result.StartTime,
                result.EndTime,
                result.IsFlagged
            });
        }


        [Authorize]
        [HttpGet]
        [Route("user-flagged")]
        public IHttpActionResult GetFlaggedUsers()
        {
            var flagged = _db.UserResults
                .Where(r => r.IsFlagged)
                .Select(r => new
                {
                    r.User.FullName,
                    r.User.Email,
                    r.ContestId,
                    r.NumberCorrect,
                    r.TotalTime,
                    r.SubmissionToken
                }).ToList();

            return Ok(flagged);
        }
    }
}