using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TodoApp.Shared.Models;
using TodoApp.Api.Options;
using Microsoft.Extensions.Options;

namespace TodoApp.Api.Services;

public class AiChatService
{
    private const string SystemPrompt = "You are a helpful AI assistant for generating todo task suggestions. Respond with 3 concise task ideas in a numbered list format, e.g., '1. Task one\\n2. Task two\\n3. Task three'. Keep it relevant to productivity.";

    private readonly HttpClient _httpClient;
    private readonly AiChatOptions _options;

    public AiChatService(HttpClient httpClient, IOptions<AiChatOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<ChatMessage> SendAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new ArgumentException("Message is required.", nameof(request));
        }

        var payload = new
        {
            model = _options.Model,
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

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        }
        
        if (!string.IsNullOrWhiteSpace(_options.Referer))
        {
            httpRequest.Headers.Add("HTTP-Referer", _options.Referer);
        }
        if (!string.IsNullOrWhiteSpace(_options.Title))
        {
            httpRequest.Headers.Add("X-Title", _options.Title);
        }

        try
        {
            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Upstream AI call failed ({(int)response.StatusCode}): {responseBody}");
            }

            // Check if response looks like JSON
            if (string.IsNullOrWhiteSpace(responseBody) || !responseBody.TrimStart().StartsWith("{"))
            {
                throw new InvalidOperationException($"Invalid response format (not JSON): {responseBody}");
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

