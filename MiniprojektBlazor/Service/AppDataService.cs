using System.Net.Http.Json;
using System.Text.Json;
using Data;

namespace Service;

public class AppDataService
{
    private readonly HttpClient http;
    private readonly IConfiguration configuration;
    private readonly string baseAPI = "";

    public AppDataService(HttpClient http, IConfiguration configuration) {
        this.http = http;
        this.configuration = configuration;
        // Denne konfiguration læses fra filen "appsettings.json".
        baseAPI = configuration["base_api"];
    }

    // -----------------------------------------------------
    // -- Questions

    public async Task<QuestionData[]?> GetQuestions() {
        var url = $"{baseAPI}questions";
        return await http.GetFromJsonAsync<QuestionData[]>(url);
    }

    public async Task<QuestionData[]?> GetQuestionsByPage(int? pageNumber, int pageSize) {
        var url = $"{baseAPI}questions/?page={pageNumber}&size={pageSize}";
        return await http.GetFromJsonAsync<QuestionData[]>(url);
    }

    public async Task<QuestionData?> GetQuestionById(int id) {
        var url = $"{baseAPI}questions/{id}";
        return await http.GetFromJsonAsync<QuestionData>(url);
    }

    public async Task<QuestionData[]?> GetQuestionsBySubjectId(int id) {
        var url = $"{baseAPI}questions/{id}/subject";
        return await http.GetFromJsonAsync<QuestionData[]>(url);
    }

    public async Task CreateQuestion(QuestionData q) {
        var data = new QuestionDataAPI(q.Subject.Id, q.Title, q.Text, q.Username);

        var url = $"{baseAPI}questions/";
        await http.PostAsJsonAsync(url, data);
    }

    public async Task UpvoteQuestion(QuestionData question) {
        var url = $"{baseAPI}questions/{question.Id}/upvote/";
        await http.PutAsJsonAsync(url, question);
    }

    public async Task DownvoteQuestion(QuestionData question) {
        var url = $"{baseAPI}questions/{question.Id}/downvote/";
        await http.PutAsJsonAsync(url, question);
    }

    // -----------------------------------------------------
    // -- Answers

    public async Task<AnswerData[]?> GetAnswersById(int id) {
        var url = $"{baseAPI}answers/{id}/question";
        return await http.GetFromJsonAsync<AnswerData[]>(url);
    }

    public async Task CreateAnswer(AnswerData a) {
        var data = new AnswerDataAPI(a.QuestionId, a.Text, a.Username);

        var url = $"{baseAPI}answers/";
        await http.PostAsJsonAsync(url, data);
    }

    public async Task UpvoteAnswer(AnswerData answer) {
        var url = $"{baseAPI}answers/{answer.Id}/upvote/";
        await http.PutAsJsonAsync(url, answer);
    }

    public async Task DownvoteAnswer(AnswerData answer) {
        var url = $"{baseAPI}answers/{answer.Id}/downvote/";
        await http.PutAsJsonAsync(url, answer);
    }

    // -----------------------------------------------------
    // -- Subjects

    public async Task<SubjectData[]?> GetSubjects() {
        var url = $"{baseAPI}subjects";
        return await http.GetFromJsonAsync<SubjectData[]>(url);
    }

    public async Task<SubjectData?> GetSubjectById(int id) {
        var url = $"{baseAPI}subjects/{id}";
        return await http.GetFromJsonAsync<SubjectData>(url);
    }

    // -----------------------------------------------------

    private record QuestionDataAPI(int SubjectId, string Title, string Text, string Username);
    private record AnswerDataAPI(int QuestionId, string Text, string Username);

    // -----------------------------------------------------
    // -- Metoder

    public string GetColor(string text) {
        var hash = 0;

        for (var i = 0; i < text.Length; i++)
        {
            hash = text.ElementAt(i) + ((hash << 5) - hash);
            hash &= hash;
        }

        var hue = hash % 360;

        if (hue < 0)
        {
            hue += 360;
        }

        var colorHsl = $"hsl({hue}deg 100% 40%)";

        return colorHsl;
    }

    public string? GetPrettyDate(DateTime d) {
        // 1.
        // Get time span elapsed since the date.
        TimeSpan s = DateTime.Now.Subtract(d);

        // 2.
        // Get total number of days elapsed.
        var dayDiff = (int)s.TotalDays;

        // 3.
        // Get total number of seconds elapsed.
        var secDiff = (int)s.TotalSeconds;

        // 4.
        // Don't allow out of range values.
        if (dayDiff < 0)
        {
            return null;
        }

        // 5.
        // Handle same-day times.
        if (dayDiff == 0)
        {
            // A.
            // Less than one minute ago.
            if (secDiff < 60)
            {
                return "lige nu";
            }
            // B.
            // Less than 2 minutes ago.
            if (secDiff < 120)
            {
                return "for 1 minut siden";
            }
            // C.
            // Less than one hour ago.
            if (secDiff < 3600)
            {
                return string.Format("for {0} minutter siden",
                    Math.Floor((double)secDiff / 60));
            }
            // D.
            // Less than 2 hours ago.
            if (secDiff < 7200)
            {
                return "for 1 time siden";
            }
            // E.
            // Less than one day ago.
            if (secDiff < 86400)
            {
                return string.Format("for {0} timer siden",
                    Math.Floor((double)secDiff / 3600));
            }
        }
        // 6.
        // Handle previous days.
        if (dayDiff == 1)
        {
            return "i går";
        }
        if (dayDiff < 7)
        {
            return string.Format("for {0} dage siden",
                dayDiff);
        }
        if (dayDiff < 31)
        {
            var weeks = Math.Ceiling((double)dayDiff / 7);

            if (weeks == 1)
            {
                return "for 1 uge siden";
            }
            else
            {
                return string.Format("for {0} uger siden",
                    weeks);
            }
        }
        if (dayDiff is > 31 and < 38)
        {
            return "for 1 måned siden";
        }
        else
        {
            return string.Format("for {0} dage siden",
                    dayDiff);
        }
    }

}