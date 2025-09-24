using System.Collections.Generic;

namespace TodoApp.Shared.Models
{
    public record ChatMessage(string Content, bool IsUser, List<string>? Suggestions = null);
}