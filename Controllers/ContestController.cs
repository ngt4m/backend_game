using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TVQD_API.Help;
using TVQD_API.Models.DTOs;

namespace TVQD_API.Controllers
{
    [RoutePrefix("api/contest")]
    public class ContestController : ApiController
    {
        private readonly AppDbContext _db = new AppDbContext();

        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> CreateContest([FromBody] CreateContestDto dto)
        {
            if (dto == null || !ModelState.IsValid)
                return BadRequest("Invalid contest data.");

            var contestId = Guid.NewGuid().ToString();

            var token = TokenHelper.GenerateAntiCheatToken(contestId);

            var contest = new Contest
            {
                Id = contestId,
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                MaxAttempts = dto.MaxAttempts,
                AntiCheatToken = token
            };

            _db.Contests.Add(contest);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Contest đã được tạo.",
                data = new
                {
                    contest.Id,
                    contest.Name,
                    contest.StartDate,
                    contest.EndDate,
                    contest.MaxAttempts,
                    contest.AntiCheatToken
                }
            });
        }


        [Authorize]
        [HttpGet]
        [Route("info/{contestIndex:int}")]
        public async Task<IHttpActionResult> GetContestInfo(int contestIndex)
        {
            // Lấy contest theo thứ tự giảm dần thời gian bắt đầu
            var contest = await _db.Contests
                .OrderByDescending(c => c.StartDate)
                .Skip(contestIndex-1)
                .FirstOrDefaultAsync();

            if (contest == null)
                return NotFound();

            // Nếu chưa có AntiCheatToken thì sinh ra
            if (string.IsNullOrEmpty(contest.AntiCheatToken))
            {
                contest.AntiCheatToken = TokenHelper.GenerateAntiCheatToken(contest.Id);
                await _db.SaveChangesAsync();
            }

            return Ok(new
            {
                contest.Id,
                contest.Name,
                contest.StartDate,
                contest.EndDate,
                contest.AntiCheatToken
            });
        }
    }

}