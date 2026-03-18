using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace KTGK.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        public string Content { get; set; }

        public int ExamId { get; set; }

        [ForeignKey("ExamId")]
        public Exam Exam { get; set; }

        public List<Answer> Answers { get; set; }

        public int? PassageId { get; set; }   // 🔥 KEY

        public Passage Passage { get; set; }
    }
}