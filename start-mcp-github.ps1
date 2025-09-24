# Start MCP GitHub Server for TodoList Project
# Pastikan file .env.mcp sudah dikonfigurasi dengan token GitHub yang valid

Write-Host "=== TodoList MCP Server Startup ===" -ForegroundColor Cyan
Write-Host "Starting MCP GitHub Server for TodoList C# Solution..." -ForegroundColor Green

# Load environment variables dari .env.mcp
if (Test-Path ".env.mcp") {
    Write-Host "Loading environment variables from .env.mcp..." -ForegroundColor Yellow
    Get-Content ".env.mcp" | ForEach-Object {
        if ($_ -match '^([^#][^=]*)=(.*)$') {
            [System.Environment]::SetEnvironmentVariable($matches[1], $matches[2], "Process")
            if ($matches[1] -eq "GITHUB_PERSONAL_ACCESS_TOKEN") {
                Write-Host "Loaded: GITHUB_PERSONAL_ACCESS_TOKEN (hidden for security)" -ForegroundColor Yellow
            } else {
                Write-Host "Loaded: $($matches[1]) = $($matches[2])" -ForegroundColor Yellow
            }
        }
    }
} else {
    Write-Error ".env.mcp file not found. Please create it with your GitHub token."
    Write-Host "Run this command to create the file:" -ForegroundColor Yellow
    Write-Host "  notepad .env.mcp" -ForegroundColor White
    exit 1
}

# Check if token is set
$token = [System.Environment]::GetEnvironmentVariable("GITHUB_PERSONAL_ACCESS_TOKEN", "Process")
if (-not $token -or $token -eq "YOUR_TOKEN_HERE") {
    Write-Error "GitHub Personal Access Token not configured in .env.mcp"
    Write-Host "Please edit .env.mcp and set your GitHub token" -ForegroundColor Yellow
    Write-Host "Example: GITHUB_PERSONAL_ACCESS_TOKEN=ghp_your_token_here" -ForegroundColor White
    exit 1
}

Write-Host "✅ Token configured successfully" -ForegroundColor Green

# Display project context
Write-Host "`n=== TodoList Project Context ===" -ForegroundColor Cyan
Write-Host "🔧 Framework: .NET 9" -ForegroundColor White
Write-Host "📁 Projects:" -ForegroundColor White
Write-Host "  • TodoApp.Api (Web API)" -ForegroundColor Gray
Write-Host "  • TodoApp.Web (Blazor WASM)" -ForegroundColor Gray
Write-Host "  • TodoApp.Shared (Class Library)" -ForegroundColor Gray
Write-Host "🤖 MCP Boost: ENABLED" -ForegroundColor Green

Write-Host "`nStarting MCP GitHub server..." -ForegroundColor Blue
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow

# Start MCP server
try {
    Write-Host "Command: mcp-server-github" -ForegroundColor Gray
    mcp-server-github
} catch {
    Write-Error "Failed to start MCP GitHub server: $_"
    Write-Host "`nTroubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Ensure mcp-server-github is installed: npm install -g @modelcontextprotocol/server-github" -ForegroundColor White
    Write-Host "2. Check your GitHub token permissions" -ForegroundColor White
    Write-Host "3. Verify .env.mcp file format" -ForegroundColor White
    exit 1
}