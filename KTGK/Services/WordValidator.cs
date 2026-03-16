using System.Text.RegularExpressions;

namespace KTGK.Services
{
    public class WordValidator
    {
        public List<string> Validate(string text)
        {
            var errors = new List<string>();

            var lines = text.Split('\n');

            int questionCount = 0;
            int answerCount = 0;

            foreach (var raw in lines)
            {
                var line = raw.Trim();

                if (Regex.IsMatch(line, @"^\d+\."))
                    questionCount++;

                if (Regex.IsMatch(line, @"^[ABCD]\."))
                    answerCount++;

                if (line.StartsWith("KEY") == false &&
                    line.StartsWith("A.") == false &&
                    line.StartsWith("B.") == false &&
                    line.StartsWith("C.") == false &&
                    line.StartsWith("D.") == false &&
                    Regex.IsMatch(line, @"^\d+\.") == false &&
                    line.Length > 0)
                {
                    // cho phép text question
                }
            }

            if (questionCount == 0)
                errors.Add("Không tìm thấy câu hỏi");

            if (answerCount < questionCount * 4)
                errors.Add("Thiếu đáp án A B C D");

            return errors;
        }
    }
}