using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace KTGK.Models
{
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }

        public string Title { get; set; }

        public int Duration { get; set; }

        public string Description { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}