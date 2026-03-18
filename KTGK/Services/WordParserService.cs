using KTGK.Data;
using KTGK.Models;
using System.IO.Packaging;
using System.Text.RegularExpressions;
using Xceed.Words.NET;

namespace KTGK.Services
{
    public class WordParserService
    {
        private readonly ApplicationDbContext _context;

        public WordParserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void ImportExam(string filePath, int examId)
        {
            using var doc = DocX.Load(filePath);
            var lines = doc.Paragraphs
                .Select(p => p.Text.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToList();

            Question currentQuestion = null;
            List<Answer> answers = new List<Answer>();

            foreach (var raw in lines)
            {
                var line = raw.Trim();

                if (string.IsNullOrEmpty(line))
                    continue;

                Console.WriteLine("LINE: " + line); // 🔥 debug

                // 🔹 Bỏ Directions
                if (line.StartsWith("Directions"))
                    continue;

                // 🔥 GẶP CÂU HỎI
                if (line.Contains("[Q:"))
                {
                    // lưu câu cũ
                    if (currentQuestion != null)
                    {
                        currentQuestion.Answers = answers;

                        if (answers.Count > 0)
                            answers[0].IsCorrect = true;

                        _context.Questions.Add(currentQuestion);
                    }

                    currentQuestion = new Question
                    {
                        Content = "", // sẽ gán ở dưới
                        ExamId = examId
                    };

                    answers = new List<Answer>();
                    continue;
                }

                // 🔹 Đáp án
                if (line.StartsWith("[A]") || line.StartsWith("[B]") ||
                    line.StartsWith("[C]") || line.StartsWith("[D]"))
                {
                    var content = line.Substring(3).Trim();

                    answers.Add(new Answer
                    {
                        Content = content,
                        IsCorrect = false
                    });

                    continue;
                }

                // 🔥 NỘI DUNG CÂU HỎI (QUAN TRỌNG NHẤT)
                if (currentQuestion != null && answers.Count == 0)
                {
                    currentQuestion.Content += line + " ";
                }
            }

            // 🔥 LƯU CÂU CUỐI
            if (currentQuestion != null)
            {
                currentQuestion.Answers = answers;

                if (answers.Count > 0)
                    answers[0].IsCorrect = true;

                _context.Questions.Add(currentQuestion);
            }

            _context.SaveChanges();
        }
    }
}