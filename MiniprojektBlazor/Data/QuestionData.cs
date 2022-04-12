using System;

namespace Data
{
    public class QuestionData
    {
        // Properties
        public int Id { get; set; }
        public SubjectData Subject { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public int Upvote { get; set; }
        public int Downvote { get; set; }

        // Konstruktør
        public QuestionData() { }
        public QuestionData(SubjectData Subject, string Title, string Text, string Username) {
            this.Subject = Subject;
            this.Title = Title;
            this.Text = Text;
            this.Username = Username;
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

    }
}
