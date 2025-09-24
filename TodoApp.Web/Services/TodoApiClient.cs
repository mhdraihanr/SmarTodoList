using System.Net.Http.Json;
using TodoApp.Api.Models;
using TodoApp.Shared;

namespace TodoApp.Web.Services;

public record CreateTodoRequest(string Title);
public record UpdateTodoRequest(string Title, bool IsDone);

public class TodoApiClient
{
    private const string BasePath = "/api/todos";
    private readonly HttpClient _http;

    public TodoApiClient(HttpClient http)
    {
        _http = http;
    }

    public Uri? BaseAddress => _http.BaseAddress;

    public async Task<List<TodoItemDto>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<TodoItemDto>>(BasePath) ?? [];

    public async Task<TodoItemDto?> GetAsync(int id)
        => await _http.GetFromJsonAsync<TodoItemDto>($"{BasePath}/{id}");

    public async Task<TodoItemDto?> CreateAsync(string title)
    {
        var res = await _http.PostAsJsonAsync(BasePath, new CreateTodoRequest(title));
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<TodoItemDto>() : null;
    }

    public async Task<bool> UpdateAsync(int id, string title, bool isDone)
    {
        var res = await _http.PutAsJsonAsync($"{BasePath}/{id}", new UpdateTodoRequest(title, isDone));
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var res = await _http.DeleteAsync($"{BasePath}/{id}");
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> ClearAllAsync()
    {
        var response = await _http.DeleteAsync(BasePath);
        return response.IsSuccessStatusCode;
    }

    public async Task<ChatMessage?> RequestChatAsync(string message, string? apiKey)
    {
        var payload = new
        {
            Message = message,
            ApiKey = apiKey
        };

        var response = await _http.PostAsJsonAsync("/api/chat", payload);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(error)
                ? $"Chat API returned {response.StatusCode}"
                : error);
        }

        return await response.Content.ReadFromJsonAsync<ChatMessage>();
    }
}
