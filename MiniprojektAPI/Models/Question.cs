using System;

namespace Models
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

        // Konstrukt�r
        public Question() { }
        public Question(Subject subject, string title, string text, string username)
        {
            Subject = subject;
            Title = title;
            Text = text;
            Username = username;
            Date = DateTime.Now;
            Upvote = 1;
            Downvote = 0;
            Answers = new List<Answer>();
        }
    }
}