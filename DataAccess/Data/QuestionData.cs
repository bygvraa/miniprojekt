using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Data
{
    public class QuestionData
    {
        // Properties
        public int Id { get; set; }

        [Required]
        [Display(Name = "Emne")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Skriv en overskrift.")]
        [Display(Name = "Overskrift")]
        [StringLength(150, ErrorMessage = "Overskriften skal være på mellem {2} og {1} tegn.", MinimumLength = 2)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Skriv et spørgsmål.")]
        [Display(Name ="Spørgsmål")]
        [StringLength(500, ErrorMessage = "Spørgsmålet skal være på mellem {2} og {1} tegn.", MinimumLength = 2)]
        public string Text { get; set; }

        [Required(ErrorMessage = "Skriv et navn.")]
        [Display(Name = "Navn")]
        [StringLength(20, ErrorMessage = "Navnet er skal være mellem {2} og {1} bogstaver langt.", MinimumLength = 2)]
        public string Username { get; set; }

        public DateTime Date { get; set; }
        public int Upvote { get; set; }
        public int Downvote { get; set; }

        // Konstruktør
        public QuestionData() { }
        public QuestionData(int Id, int SubjectId, string Title, string Text, string Username) {
            this.Id = Id;
            this.SubjectId = SubjectId;
            this.Title = Title;
            this.Text = Text;
            this.Username = Username;
        }
        public QuestionData(int SubjectId, string Title, string Text, string Username) {
            this.SubjectId = SubjectId;
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

        public string GetPrettyName() {
            return char.ToUpper(Username[0]) + Username[1..].ToLower();
        }

    }
}
