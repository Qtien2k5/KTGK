using Xceed.Words.NET;
using KTGK.Models;
using System.Text.RegularExpressions;

namespace KTGK.Parsers
{
    public class ToeicWordParser
    {
        public List<Question> Parse(string filePath)
        {
            var questions = new List<Question>();

            using (var doc = DocX.Load(filePath))
            {
                var lines = doc.Text.Split('\n');

                Question currentQuestion = null;

                foreach (var raw in lines)
                {
                    var line = raw.Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    // 🔹 [Q:101]
                    if (Regex.IsMatch(line, @"^\[Q:\d+\]"))
                    {
                        if (currentQuestion != null)
                            questions.Add(currentQuestion);

                        currentQuestion = new Question
                        {
                            Content = "",
                            Answers = new List<Answer>()
                        };
                    }

                    // 🔹 [A] [B] [C] [D]
                    else if (Regex.IsMatch(line, @"^\[[A-D]\]"))
                    {
                        if (currentQuestion == null) continue;

                        var content = line.Substring(3).Trim();

                        currentQuestion.Answers.Add(new Answer
                        {
                            Content = content,
                            IsCorrect = false
                        });
                    }

                    // 🔹 [KEY:B]
                    else if (Regex.IsMatch(line, @"^\[KEY:[A-D]\]"))
                    {
                        if (currentQuestion == null) continue;

                        var key = line.Replace("[KEY:", "").Replace("]", "").Trim();

                        int index = key[0] - 'A';

                        if (index >= 0 && index < currentQuestion.Answers.Count)
                        {
                            currentQuestion.Answers[index].IsCorrect = true;
                        }
                    }

                    // 🔹 Nội dung câu hỏi
                    else
                    {
                        if (currentQuestion != null && currentQuestion.Answers.Count == 0)
                        {
                            currentQuestion.Content += line + " ";
                        }
                    }
                }

                // add câu cuối
                if (currentQuestion != null)
                    questions.Add(currentQuestion);
            }

            return questions;
        }
    }
}