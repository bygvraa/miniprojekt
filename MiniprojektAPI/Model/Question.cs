using System;

namespace Model
{
    public class Question
    {
        // Propterties
        public int Id { get; set; }
        public Subject Subject { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public int Upvote { get; set; }
        public int Downvote { get; set; }

        public List<Answer> Answers { get; set; }

        // Konstruktører
        public Question() { }

        public Question(Subject subject, string title, string text, string username, DateTime date)
        {
            this.Subject = subject;
            this.Title = title;
            this.Text = text;
            this.Username = username;
            this.Date = date;
        }
    }
}