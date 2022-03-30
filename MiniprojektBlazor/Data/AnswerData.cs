namespace Data
{
    public class AnswerData
    {
        public AnswerData() { }

        public AnswerData(int QuestionId, string Text, string Username)
        {
            this.QuestionId = QuestionId;
            this.Text = Text;
            this.Username = Username;
        }

        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public int Upvote { get; set; }
        public int Downvote { get; set; }

        // Metoder
        public int GetScore()
        {
            return Upvote - Downvote;
        }
    }

}
