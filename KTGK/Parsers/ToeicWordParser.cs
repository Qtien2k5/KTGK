using Xceed.Words.NET;
using KTGK.Models;

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
                List<Answer> answers = new List<Answer>();

                foreach (var raw in lines)
                {
                    var line = raw.Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    // Question
                    if (char.IsDigit(line[0]))
                    {
                        currentQuestion = new Question
                        {
                            Content = line,
                            Answers = new List<Answer>()
                        };

                        questions.Add(currentQuestion);
                        answers.Clear();
                    }

                    // Answers
                    else if (line.StartsWith("A.") ||
                             line.StartsWith("B.") ||
                             line.StartsWith("C.") ||
                             line.StartsWith("D."))
                    {
                        var answer = new Answer
                        {
                            Content = line.Substring(2).Trim(),
                            IsCorrect = false
                        };

                        currentQuestion.Answers.Add(answer);
                        answers.Add(answer);
                    }

                    // KEY
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
            }

            return questions;
        }
    }
}