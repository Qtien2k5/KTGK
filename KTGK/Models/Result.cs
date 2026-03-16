using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTGK.Models
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        public string UserName { get; set; }

        public int Score { get; set; }

        public int ExamId { get; set; }

        [ForeignKey("ExamId")]
        public Exam Exam { get; set; }
    }
}