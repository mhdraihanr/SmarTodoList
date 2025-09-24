using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TodoApp.Api.Models;

namespace TodoApp.Api.Services;

public class AiChatService
{
    private const string SystemPrompt = "You are a helpful AI assistant for generating todo task suggestions. Respond with 3 concise task ideas in a numbered list format, e.g., '1. Task one\\n2. Task two\\n3. Task three'. Keep it relevant to productivity.";
    private const string RefererHeader = "https://todolist.example.com";
    private const string TitleHeader = "TodoApp AI Chat";
    private const string DefaultApiKey = "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";

    private readonly HttpClient _httpClient;

    public AiChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ChatMessage> SendAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new ArgumentException("Message is required.", nameof(request));
        }

        var payload = new
        {
            model = "x-ai/grok-4-fast:free",
            messages = new[]
                {
                new { role = "system", content = SystemPrompt },
                new { role = "user", content = request.Message }
            },
            max_tokens = 150,
            temperature = 0.7
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/chat/completions")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        var apiKey = string.IsNullOrWhiteSpace(request.ApiKey) ? DefaultApiKey : request.ApiKey;
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        httpRequest.Headers.Add("HTTP-Referer", RefererHeader);
        httpRequest.Headers.Add("X-Title", TitleHeader);

        try
        {
            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Upstream AI call failed ({(int)response.StatusCode}): {responseBody}");
            }

            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;
            var aiContent = root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "No response from AI.";

            var suggestions = ParseSuggestions(aiContent);
            return new ChatMessage(aiContent, false, suggestions);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("Tidak dapat terhubung ke layanan AI upstream. Periksa koneksi jaringan atau kredensial API.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new InvalidOperationException("Permintaan ke layanan AI melebihi batas waktu.", ex);
        }
    }

    private static List<string> ParseSuggestions(string response)
    {
        var suggestions = new List<string>();
        var lines = response.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("1.") || trimmed.StartsWith("2.") || trimmed.StartsWith("3."))
            {
                var suggestion = trimmed.Length > 2 ? trimmed[2..].Trim() : string.Empty;
                if (!string.IsNullOrEmpty(suggestion) && suggestion.Length > 5)
                {
                    suggestions.Add(suggestion);
                }
            }
        }

        return suggestions.Take(3).ToList();
    }
}

