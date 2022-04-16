namespace Data
{
    public class AnswerData
    {
        // Properties
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public int Upvote { get; set; }
        public int Downvote { get; set; }

        // Konstruktør
        public AnswerData() { }
        public AnswerData(int QuestionId, string Text, string Username) {
            this.QuestionId = QuestionId;
            this.Text = Text;
            this.Username = Username;
        }

        // Metoder
        public int GetScore() {
            return Upvote - Downvote;
        }

        public string GetPrettyName() {
            return char.ToUpper(Username[0]) + Username[1..].ToLower();
        }
    }

}
