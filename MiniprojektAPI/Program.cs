using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;

using API.Data;
using Service;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions()
    {
        WebRootPath = "wwwroot"
    }
);

// Tilføj 'DataService' så den kan bruges i endpoints
builder.Services.AddScoped<DataService>();


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
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuestionContextSql")));

// Kan vise flotte fejlbeskeder i browseren, hvis der kommer fejl fra databasen
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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

app.MapGet("/api/questions/", async (DataService service, int page, int size) =>
{
    return await service.GetQuestionsByPage(page, size);
})
    .WithName("GetQuestionsByPage").WithTags("Questions");


app.MapGet("/api/questions/{id}", async (DataService service, int id) =>
{
    return await service.GetQuestionById(id);
})
    .WithName("GetQuestionById").WithTags("Questions");


app.MapGet("/api/questions/{id}/subject", async (DataService service, int id) =>
{
    return await service.GetQuestionsBySubjectId(id);
})
    .WithName("GetQuestionsBySubject").WithTags("Questions");


app.MapPost("/api/questions/", async (DataService service, QuestionData data) =>
{
    return await service.CreateQuestion(data.SubjectId, data.Title, data.Text, data.Username);
})
    .WithName("CreateQuestion").WithTags("Questions");


app.MapPut("/api/questions/{id}/upvote", async (DataService service, int id) =>
{
    return await service.UpdateQuestionByIdUpvote(id);
})
    .WithName("UpvoteQuestion").WithTags("Questions");


app.MapPut("/api/questions/{id}/downvote", async (DataService service, int id) =>
{
    return await service.UpdateQuestionByIdDownvote(id);
})
    .WithName("DownvoteQuestion").WithTags("Questions");


// ---------------------------------------------------------------
// -- Answers --

app.MapGet("/api/answers/{id}/question", async (DataService service, int id) =>
{
    return await service.GetAnswersByQuestionId(id);
})
    .WithName("GetAnswers").WithTags("Answers");


app.MapPost("/api/answers/", async (DataService service, AnswerData data) =>
{
    return await service.CreateAnswer(data.QuestionId, data.Text, data.Username);
})
    .WithName("CreateAnswer").WithTags("Answers");


app.MapPut("/api/answers/{id}/upvote", async (DataService service, int id) =>
{
    return await service.UpdateAnswerByIdUpvote(id);
})
    .WithName("UpvoteAnswer").WithTags("Answers");


app.MapPut("/api/answers/{id}/downvote", async (DataService service, int id) =>
{
    return await service.UpdateAnswerByIdDownvote(id);
})
    .WithName("DownvoteAnswer").WithTags("Answers");


// ---------------------------------------------------------------
// -- Subjects --

app.MapGet("/api/subjects/", async (DataService service) =>
{
    return await service.GetSubjects();
})
    .WithName("GetSubjects").WithTags("Subjects");


app.MapGet("/api/subjects/{id}", async (DataService service, int id) =>
{
    return await service.GetSubjectById(id);
})
    .WithName("GetSubjectById").WithTags("Subjects");


// ---------------------------------------------------------------

app.Run();

// -- Records ----------------------------------------------------
internal record QuestionData(int SubjectId, string Title, string Text, string Username);
internal record AnswerData(int QuestionId, string Text, string Username);
internal record SubjectData(string Name);