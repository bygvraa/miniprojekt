namespace DataAccess.Models
{
    public class Answer
    {
        // Properties
        public int Id { get; set; }
        public Question Question { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public int Upvote { get; set; }
        public int Downvote { get; set; }

        // Konstruktør
        public Answer() { }
        public Answer(Question question, string text, string username) {
            Question = question;
            Text = text;
            Username = username;
            Date = DateTime.Now;
            Upvote = 1;
            Downvote = 0;
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