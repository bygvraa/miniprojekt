using System;

namespace DataAccess.Models
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

        // Konstruktør
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

        // Metoder
        public int GetScore() {
            return Upvote - Downvote;
        }

        public string GetTotalVotes() {
            var votes = Upvote + Downvote;

            if (votes == 1)
                return $"1 stemme";
            else
                return $"{votes} stemmer";
        }

        public string GetShortText(int limit) {
            if (Text.Length < limit)
                return $"{Text}";
            else
                return $"{Text[..limit]} ...";
        }

        public string GetPrettyName() {
            return char.ToUpper(Username[0]) + Username[1..].ToLower();
        }
    }
}