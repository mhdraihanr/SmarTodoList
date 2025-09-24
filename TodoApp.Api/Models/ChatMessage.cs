using System.Collections.Generic;

namespace TodoApp.Api.Models
{
    public record ChatMessage(string Content, bool IsUser, List<string>? Suggestions = null);
}
