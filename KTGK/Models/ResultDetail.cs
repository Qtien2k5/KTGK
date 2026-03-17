namespace KTGK.Models
{
    public class ResultDetail
    {
        public int ResultDetailId { get; set; }

        public int ResultId { get; set; }

        public int QuestionId { get; set; }

        public string SelectedAnswer { get; set; }

        public string CorrectAnswer { get; set; }
    }
}