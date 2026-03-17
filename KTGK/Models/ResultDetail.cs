using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTGK.Models
{
    public class ResultDetail
    {
        [Key]
        public int Id { get; set; }

        public int ResultId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }

        [ForeignKey("ResultId")]
        public Result Result { get; set; }

        [ForeignKey("QuestionId")]
        public Question Question { get; set; }
    }
}