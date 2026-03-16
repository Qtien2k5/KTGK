using KTGK.Data;
using KTGK.Models;
using KTGK.Parsers;
using Xceed.Words.NET;

namespace KTGK.Services
{
    public class WordParserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ToeicWordParser _parser;
        private readonly WordValidator _validator;

        public WordParserService(ApplicationDbContext context)
        {
            _context = context;
            _parser = new ToeicWordParser();
            _validator = new WordValidator();
        }

        public string ImportExam(string filePath)
        {
            using var doc = DocX.Load(filePath);

            var text = doc.Text;

            var errors = _validator.Validate(text);

            if (errors.Count > 0)
            {
                return string.Join("\n", errors);
            }

            var questions = _parser.Parse(filePath);

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var exam = new Exam
                {
                    Title = "Imported TOEIC Exam",
                    Duration = 75,
                    Description = "Imported from Word"
                };

                _context.Exams.Add(exam);
                _context.SaveChanges();

                foreach (var q in questions)
                {
                    q.ExamId = exam.ExamId;

                    _context.Questions.Add(q);
                    _context.SaveChanges();

                    foreach (var a in q.Answers)
                    {
                        a.QuestionId = q.QuestionId;
                        _context.Answers.Add(a);
                    }
                }

                _context.SaveChanges();

                transaction.Commit();

                return "Import thành công";
            }
            catch
            {
                transaction.Rollback();
                return "Import thất bại - rollback database";
            }
        }
    }
}