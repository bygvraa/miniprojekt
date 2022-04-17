using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System;

using API.Data;
using DataAccess.Models;

namespace Service;

public class DataService
{
    private ApplicationDbContext db { get; }

    public DataService(ApplicationDbContext db) {
        this.db = db;
    }

    // ---------------------------------------------------------------
    // -- Questions --

    public async Task<List<Question>> GetQuestions() {
        return await db.Questions
            .Include(q => q.Subject)
            .ToListAsync();
    }

    public async Task<List<Question>> GetQuestionsByPage(int page, int size) {
        return await db.Questions
            .Include(q => q.Subject)
            .OrderByDescending(q => q.Date)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<Question> GetQuestionById(int questionId) {
        return await db.Questions
            .Where(q => q.Id == questionId)
            .Include(q => q.Subject)
            .FirstAsync();
    }

    public async Task<List<Question>> GetQuestionsBySubjectId(int subjectId) {
        Subject subject = await db.Subjects
            .Where(s => s.Id == subjectId)
            .FirstAsync();

        return db.Questions
            .Where(q => q.Subject == subject)
            .ToList();
    }

    public async Task<string> CreateQuestion(int subjectId, string title, string text, string username) {
        Subject subject = await db.Subjects
            .Where(s => s.Id == subjectId)
            .FirstAsync();

        Question question = new(subject, title, text, username);

        await db.Questions.AddAsync(question);
        await db.SaveChangesAsync();

        return JsonSerializer.Serialize(
            new { msg = "New question created", newQuestion = question.Id, question.Title }
        );
    }

    public async Task<string> UpdateQuestionByIdUpvote(int questionId) {
        Question question = await db.Questions
            .Where(q => q.Id == questionId)
            .FirstAsync();

        question.Upvote++;

        db.Questions.Update(question);
        await db.SaveChangesAsync();

        return JsonSerializer.Serialize(
            new { msg = "Question upvoted", updatedQuestion = question.Title }
        );
    }

    public async Task<string> UpdateQuestionByIdDownvote(int questionId) {
        Question question = await db.Questions
            .Where(q => q.Id == questionId)
            .FirstAsync();

        question.Downvote++;

        db.Questions.Update(question);
        await db.SaveChangesAsync();

        return JsonSerializer.Serialize(
            new { msg = "Question downvoted", updatedQuestion = question.Title }
        );
    }

    // ---------------------------------------------------------------
    // -- Subjects --

    public async Task<List<Subject>> GetSubjects() {
        return await db.Subjects
            .ToListAsync();
    }

    public async Task<Subject> GetSubjectById(int subjectId) {
        return await db.Subjects
            .Where(s => s.Id == subjectId)
            .FirstAsync();
    }

    // ---------------------------------------------------------------
    // -- Answers --

    public async Task<List<Answer>> GetAnswersByQuestionId(int questionId) {
        return await db.Questions
            .Where(q => q.Id == questionId)
            .SelectMany(q => q.Answers)
            .ToListAsync();
    }

    public async Task<string> CreateAnswer(int questionId, string text, string username) {
        Question question = await db.Questions
            .Where(q => q.Id == questionId)
            .FirstAsync();

        Answer answer = new(question, text, username);

        await db.Answers.AddAsync(answer);
        await db.SaveChangesAsync();

        return JsonSerializer.Serialize(
            new { msg = "New answer created", newAnswer = answer.Text }
        );
    }

    public async Task<string> UpdateAnswerByIdUpvote(int answerId) {
        Answer answer = await db.Answers
            .Where(a => a.Id == answerId)
            .FirstAsync();

        answer.Upvote++;

        db.Answers.Update(answer);
        await db.SaveChangesAsync();

        return JsonSerializer.Serialize(
            new { msg = "Answer upvoted", updatedAnswer = answer.Text }
        );
    }

    public async Task<string> UpdateAnswerByIdDownvote(int answerId) {
        Answer answer = await db.Answers
            .Where(a => a.Id == answerId)
            .FirstAsync();

        answer.Downvote++;

        db.Answers.Update(answer);
        await db.SaveChangesAsync();

        return JsonSerializer.Serialize(
            new { msg = "Answer downvoted", updatedAnswer = answer.Text }
        );
    }

    // ---------------------------------------------------------------

    /// <summary>
    /// Seeder noget nyt data i databasen hvis det er nødvendigt.
    /// </summary>
	public void SeedData() {
        Subject subject = db.Subjects.FirstOrDefault()!;
        if (subject == null)
        {
            subject = new Subject("Teknisk");
            db.Subjects.Add(subject);
            db.Subjects.Add(new Subject("Hjælp"));
            db.Subjects.Add(new Subject("Mad"));
            db.Subjects.Add(new Subject("Andet"));
        }

        Question question = db.Questions.FirstOrDefault()!;
        if (question == null)
        {
            question = new Question(subject, "Teknisk spørgsmål", "Virker mit spørgsmål?", "Mikkel");
            db.Questions.Add(question);
        }

        Answer answer = db.Answers.FirstOrDefault()!;
        if (answer == null)
        {
            answer = new Answer(question, "Ja, dit spørgsmål virker", "Simon");
            db.Answers.Add(answer);
        }

        db.SaveChanges();
    }
}
