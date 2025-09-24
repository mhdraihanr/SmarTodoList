using System.Net;
using TodoApp.Api.Models;
using TodoApp.Api.Services;
using TodoApp.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Allow FE (Blazor WASM) to call API
var corsPolicy = "_allowWeb";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(corsPolicy, p =>
        p.AllowAnyOrigin()       // untuk produksi: batasi ke origin FE
         .AllowAnyHeader()
         .AllowAnyMethod());
});



// DI
builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
builder.Services.AddHttpClient<AiChatService>(client =>
{
    client.BaseAddress = new Uri("https://openrouter.ai");
});

var app = builder.Build();

app.UseCors(corsPolicy);

if (app.Environment.IsDevelopment())
{

}
app.MapPost("/api/chat", async (ChatRequest request, AiChatService chatService, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Message))
    {
        return Results.BadRequest("Message is required.");
    }

    try
    {
        var aiMessage = await chatService.SendAsync(request, cancellationToken);
        return Results.Ok(aiMessage);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Problem(ex.Message, statusCode: (int)HttpStatusCode.BadGateway);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Unexpected error while contacting AI service: {ex.Message}");
    }
});

app.MapGet("/api/todos", (ITodoRepository repo) =>
{
    var list = repo.GetAll().Select(x => new TodoItemDto(x.Id, x.Title, x.IsDone, x.CreatedAt));
    return Results.Ok(list);
});

app.MapGet("/api/todos/{id:int}", (int id, ITodoRepository repo) =>
{
    var x = repo.Get(id);
    return x is null
        ? Results.NotFound()
        : Results.Ok(new TodoItemDto(x.Id, x.Title, x.IsDone, x.CreatedAt));
});

app.MapPost("/api/todos", (CreateTodoRequest req, ITodoRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(req.Title)) return Results.BadRequest("Title required");
    var saved = repo.Add(new TodoItem { Title = req.Title });
    return Results.Created($"/api/todos/{saved.Id}",
        new TodoItemDto(saved.Id, saved.Title, saved.IsDone, saved.CreatedAt));
});

app.MapPut("/api/todos/{id:int}", (int id, UpdateTodoRequest req, ITodoRepository repo) =>
{
    var exist = repo.Get(id);
    if (exist is null) return Results.NotFound();
    exist.Title = req.Title;
    exist.IsDone = req.IsDone;
    return repo.Update(exist) ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/api/todos/{id:int}", (int id, ITodoRepository repo) =>
{
    return repo.Delete(id) ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/api/todos", (ITodoRepository repo) =>
{
    repo.ClearAll();
    return Results.NoContent();
});

app.Run();


