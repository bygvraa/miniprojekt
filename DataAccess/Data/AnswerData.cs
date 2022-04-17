using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Data
{
    public class AnswerData
    {
        // Properties
        public int Id { get; set; }
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Skriv et svar.")]
        [Display(Name = "Svar")]
        [StringLength(500, ErrorMessage = "Svaret skal være på mellem {2} og {1} tegn.", MinimumLength = 2)]
        public string Text { get; set; }

        [Required(ErrorMessage = "Skriv et navn.")]
        [Display(Name = "Navn")]
        [StringLength(20, ErrorMessage = "Navnet skal være mellem {2} og {1} bogstaver langt.", MinimumLength = 2)]
        public string Username { get; set; }

        public DateTime Date { get; set; }
        public int Upvote { get; set; }
        public int Downvote { get; set; }

        // Konstruktør
        public AnswerData() { }
        public AnswerData(int Id, int QuestionId, string Text, string Username) {
            this.Id = Id;
            this.QuestionId = QuestionId;
            this.Text = Text;
            this.Username = Username;
        }
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
