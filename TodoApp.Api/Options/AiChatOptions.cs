namespace TodoApp.Api.Options;

public class AiChatOptions
{
    public string ApiBaseUrl { get; set; } = "https://openrouter.ai";
    public string Model { get; set; } = "x-ai/grok-4-fast:free";
    public string Referer { get; set; } = "https://todolist.example.com";
    public string Title { get; set; } = "TodoApp AI Chat";
    // DO NOT commit secrets to source control; use user-secrets or environment vars
    public string? ApiKey { get; set; }
}
