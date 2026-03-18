using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTGK.Models
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }
        public string StudentName { get; set; }
        public int Score { get; set; }
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public DateTime SubmitTime { get; set; }
        public int TimeTaken { get; set; } // ← THÊM: giây đã làm bài

        [ForeignKey("ExamId")]
        public Exam Exam { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}