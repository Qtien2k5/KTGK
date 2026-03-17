using KTGK.Data;
using KTGK.Models;
using Xceed.Words.NET;

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

        var lines = doc.Text.Split('\n');

        Question currentQuestion = null;
        List<Answer> answers = new List<Answer>();

        foreach (var raw in lines)
        {
            var line = raw.Trim();

            if (string.IsNullOrEmpty(line))
                continue;

            // Câu hỏi: bắt đầu bằng số (101, 102...)
            if (char.IsDigit(line[0]))
            {
                currentQuestion = new Question
                {
                    Content = line,
                    ExamId = examId,
                    Answers = new List<Answer>()
                };

                _context.Questions.Add(currentQuestion);
                _context.SaveChanges();
            }

            // Đáp án
            else if (line.StartsWith("A.") ||
                     line.StartsWith("B.") ||
                     line.StartsWith("C.") ||
                     line.StartsWith("D."))
            {
                var answer = new Answer
                {
                    Content = line.Substring(2).Trim(),
                    QuestionId = currentQuestion.QuestionId,
                    IsCorrect = false
                };

                _context.Answers.Add(answer);
                answers.Add(answer);
            }

            // KEY: A/B/C/D
            else if (line.StartsWith("KEY"))
            {
                var key = line.Split(':')[1].Trim();
                int index = key[0] - 'A';

                if (index >= 0 && index < answers.Count)
                {
                    answers[index].IsCorrect = true;
                }
            }
        }

        _context.SaveChanges();
    }

    private void SaveQuestion(string content, int examId)
    {
        var question = new Question
        {
            Content = content.Trim(),
            ExamId = examId
        };

        _context.Questions.Add(question);
    }
}