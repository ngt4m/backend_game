using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace TVQD_API.Help
{

    public static class ProfanityHelper
    {
        private static readonly HashSet<string> BadWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        // 🔞 Từ tục liên quan đến tình dục
        "địt", "đụ", "cặc", "lồn", "buồi", "bướm", "dái", "cu", "chim", "nứng", "xoạc", "phê", "bú", "húp", "đéo",
        "chịch", "thẩm", "tự sướng", "sục", "thụt", "vãi", "vãi lồn", "vãi cặc", "vãi buồi", "bắn tinh", "bú lol",

        // 🤬 Từ xúc phạm, chửi rủa
        "mẹ", "mẹ mày", "mẹ cha", "mẹ kiếp", "mẹ nó", "con mẹ", "bố", "bố láo", "bố đời",
        "đồ chó", "chó", "óc chó", "mặt chó", "mặt lồn", "thằng chó", "chó má", "khốn", "khốn nạn", "khốn kiếp",
        "mất dạy", "láo", "láo toét", "láo toét", "ngu", "ngu lồn", "ngu vãi", "ngu vcl", "rảnh háng", "vô học",

        // 🧑‍🎤 Từ chỉ gái mại dâm
        "đĩ", "điếm", "cave", "con đĩ", "con điếm", "gái gọi", "bán hoa",

        // 🚫 Từ lách viết tắt, biến thể
        "vcl", "vl", "cl", "clgt", "dm", "dmm", "đcm", "đkm", "bml", "thml", "ditme", "memay", "mekiep", "concho", "occho"
    };


        private static readonly List<Regex> ProfanityRegexes = new List<Regex>
    {
        new Regex(@"\bd[\*\.\s]?i[\*\.\s]?t\b", RegexOptions.IgnoreCase),        // đ*ịt, đ.i.t
        new Regex(@"\bd[\*\.\s]?ụ\b", RegexOptions.IgnoreCase),                 // đụ
        new Regex(@"\bc[\*\.\s]?[@aạâă]c\b", RegexOptions.IgnoreCase),          // cặc, c@c
        new Regex(@"\bl[o0ôơöø]{1,2}n\b", RegexOptions.IgnoreCase),            // l0n, lôn
        new Regex(@"\bb[uư]{1,2}ồi\b", RegexOptions.IgnoreCase),               // buồi
        new Regex(@"\bd[. ]?m\b", RegexOptions.IgnoreCase),                    // đ.m
        new Regex(@"\bd[. ]?m[ẹeê]?", RegexOptions.IgnoreCase),                // đ.mẹ, đ mẹ
        new Regex(@"\bm[e3][kki]i[e3]p\b", RegexOptions.IgnoreCase),           // mekiep
        new Regex(@"\bm[e3][m4]ay\b", RegexOptions.IgnoreCase),                // mẹ mày
        new Regex(@"\bd[i1][t7][m3][e3]\b", RegexOptions.IgnoreCase),          // ditme
        new Regex(@"\bconch[o0]\b", RegexOptions.IgnoreCase),                  // concho
        new Regex(@"\bocch[o0]\b", RegexOptions.IgnoreCase),                   // occho
        new Regex(@"\bvl\b", RegexOptions.IgnoreCase),
        new Regex(@"\bvcl\b", RegexOptions.IgnoreCase),
        new Regex(@"\bclgt\b", RegexOptions.IgnoreCase),
    };


        /// <summary>
        /// Kiểm tra từng từ trong chuỗi đầu vào, nếu có từ tục hoặc viết lệch → trả về true
        /// </summary>
        public static bool ContainsProfanity(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Chuẩn hóa: về thường, bỏ dấu câu
            string normalized = NormalizeInput(input);

            // Tách từ theo khoảng trắng
            var words = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                // 1. Kiểm tra theo danh sách từ
                if (BadWords.Contains(word))
                    return true;

                // 2. Kiểm tra theo regex viết lệch
                foreach (var pattern in ProfanityRegexes)
                {
                    if (pattern.IsMatch(word))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Bỏ các ký tự đặc biệt, giữ lại khoảng trắng
        /// </summary>
        private static string NormalizeInput(string input)
        {
            return Regex.Replace(input.ToLowerInvariant(), @"[^\p{L}\p{N}\s]", " ");
        }
    }

}