using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;

using Data;
using Service;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions()
    {
        WebRootPath = "wwwroot"
    }
);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS skal slåes til i app'en. Ellers kan man ikke hente data fra et andet domæne.
// Se mere her: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0
var AllowSomeStuff = "_AllowSomeStuff";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSomeStuff, builder => {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Tilføj DbContext factory som service, så man kan få context ind via Dependency Injection.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("QuestionContextSQLite")));

// Kan vise flotte fejlbeskeder i browseren, hvis der kommer fejl fra databasen
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Tilføj 'DataService' så den kan bruges i endpoints
builder.Services.AddScoped<DataService>();

// Her kan man styre, hvordan den laver JSON.
builder.Services.Configure<JsonOptions>(options =>
{
    // Super vigtig option! Den gør, at programmet ikke smider fejl,
    // når man returnerer JSON med objekter, der refererer til hinanden.
    // (altså dobbelrettede associeringer)
    options.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});



// ---------------------------------------------------------------

var app = builder.Build();

// Sørg for at HTML mv. også kan serveres
var options = new DefaultFilesOptions();
options.DefaultFileNames.Clear();
options.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(options);
app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true
});

// Seeding af data, hvis databasen er tom
using (var scope = app.Services.CreateScope())
{
    // Med 'scope' kan man hente en service.
    var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
    dataService.SeedData(); // 'SeedData()' er defineret i 'DataService.cs', og fylder data på databasen, hvis den er tom.
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseCors(AllowSomeStuff);

app.UseRouting();

app.MapFallbackToFile("index.html");

// Middlware der kører før hver request. Alle svar skal have ContentType: JSON.
app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});

// Endpoints i API'en --------------------------------------------
// ---------------------------------------------------------------
// -- Questions --

app.MapGet("/api/questions/", (DataService service) =>
{
    return service.ListQuestions();
});

app.MapGet("/api/questions/{id}", (DataService service, int id) =>
{
    return service.GetQuestionById(id);
});

app.MapGet("/api/questions/{id}/subject", (DataService service, int id) =>
{
    return service.ListQuestionsBySubjectId(id);
});

app.MapPost("/api/questions/", (DataService service, QuestionDataAPI data) =>
{
    return service.CreateQuestion(data.SubjectId, data.Title, data.Text, data.Username);
});

app.MapPut("/api/questions/{id}/upvote", (DataService service, int id) =>
{
    return service.UpdateQuestionByIdUpvote(id);
});

app.MapPut("/api/questions/{id}/downvote", (DataService service, int id) =>
{
    return service.UpdateQuestionByIdDownvote(id);
});

// ---------------------------------------------------------------
// -- Subjects --

app.MapGet("/api/subjects/", (DataService service) =>
{
    return service.ListSubjects();
});

app.MapGet("/api/subjects/{id}", (DataService service, int id) =>
{
    return service.GetSubjectById(id);
});

// ---------------------------------------------------------------
// -- Answers --

app.MapGet("/api/answers/{id}/question", (DataService service, int id) =>
{
    return service.ListAnswersByQuestionId(id);
});

app.MapPost("/api/answers/", (DataService service, AnswerDataAPI data) =>
{
    return service.CreateAnswer(data.QuestionId, data.Text, data.Username);
});

app.MapPut("/api/answers/{id}/upvote", (DataService service, int id) =>
{
    return service.UpdateAnswerByIdUpvote(id);
});

app.MapPut("/api/answers/{id}/downvote", (DataService service, int id) =>
{
    return service.UpdateAnswerByIdDownvote(id);
});

// ---------------------------------------------------------------

app.Run();

// -- Records ----------------------------------------------------

internal record QuestionDataAPI(int SubjectId, string Title, string Text, string Username);
internal record SubjectDataAPI(string Name);
internal record AnswerDataAPI(int QuestionId, string Text, string Username);