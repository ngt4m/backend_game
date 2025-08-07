using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TVQD_API.Help
{
    public static class TokenHelper
    {
        public static string GenerateAntiCheatToken(string contestId)
        {
            string secretKey = ConfigurationManager.AppSettings["AntiCheatSecret"];

            string input = contestId + DateTime.Now.Ticks + secretKey;

            return Sha256Hash(input);
        }

        public static string GenerateSubmissionToken(string userId, string contestId, DateTime startTime)
        {
            string secretKey = ConfigurationManager.AppSettings["SubmissionSecret"];
            string raw = userId + contestId + startTime.Ticks + secretKey;
            return Sha256Hash(raw);
        }

        private static string Sha256Hash(string raw)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(raw);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }

}