using Microsoft.EntityFrameworkCore;
using NoteTakingAPI.Data;
using NoteTakingAPI.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// SQLite Database (persistent)
builder.Services.AddDbContext<NoteDbContext>(options => options.UseSqlite("Data Source=notes.db"));

// Configure HttpClient for OpenAI API
builder.Services.AddHttpClient();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Detailed error page during development
}

// Routes
app.MapPost("/notes", async (Note note, NoteDbContext db, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];

    if (string.IsNullOrEmpty(openAiApiKey))
    {
        return Results.Problem(detail: "API key is missing", statusCode: 401);
    }

    // Set authorization header for OpenAI API
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiApiKey}");

    // Generate Summary using OpenAI API
    var requestBody = new
    {
        model = "gpt-3.5-turbo-instruct",
        prompt = $"Summarize this note: {note.Content}",
        max_tokens = 50
    };

    var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
    var content = new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json");

    var response = await client.PostAsync("https://api.openai.com/v1/completions", content);

    if (!response.IsSuccessStatusCode)
    {
        var errorDetail = await response.Content.ReadAsStringAsync();
        Console.Error.WriteLine(errorDetail);
        return Results.Problem(detail: errorDetail, statusCode: (int)response.StatusCode);
    }

    var responseBody = await response.Content.ReadAsStringAsync();
    var summary = JsonConvert.DeserializeObject<dynamic>(responseBody)?.choices[0]?.text.ToString().Trim();

    note.Summary = summary;
    note.CreatedAt = DateTime.UtcNow;
    note.UpdatedAt = DateTime.UtcNow;

    db.Notes.Add(note);
    await db.SaveChangesAsync();
    return Results.Created($"/notes/{note.Id}", note);
});

app.MapGet("/notes", async (NoteDbContext db) =>
    await db.Notes.ToListAsync());

app.MapGet("/notes/{id}", async (int id, NoteDbContext db) =>
    await db.Notes.FindAsync(id)
        is Note note
            ? Results.Ok(note)
            : Results.NotFound());

app.MapPut("/notes/{id}", async (int id, Note updatedNote, NoteDbContext db, IHttpClientFactory httpClientFactory) =>
{
    var note = await db.Notes.FindAsync(id);

    if (note == null) return Results.NotFound();

    note.Title = updatedNote.Title;
    note.Content = updatedNote.Content;

    // Regenerate Summary if Body changes using OpenAI API
    var client = httpClientFactory.CreateClient();
    var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiApiKey}");

    var requestBody = new
    {
        model = "gpt-3.5-turbo-instruct",
        prompt = $"Summarize this note: {note.Content}",
        max_tokens = 50
    };

    var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
    var content = new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json");

    var response = await client.PostAsync("https://api.openai.com/v1/completions", content);

    if (!response.IsSuccessStatusCode)
    {
        return Results.Problem(
            detail: await response.Content.ReadAsStringAsync(),
            statusCode: (int)response.StatusCode
        );
    }

    var responseBody = await response.Content.ReadAsStringAsync();
    var summary = JsonConvert.DeserializeObject<dynamic>(responseBody)?.choices[0]?.text.ToString().Trim();

    note.Summary = summary;
    note.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(note);
});

app.MapDelete("/notes/{id}", async (int id, NoteDbContext db) =>
{
    if (await db.Notes.FindAsync(id) is Note note)
    {
        db.Notes.Remove(note);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();
