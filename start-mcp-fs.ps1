# Start MCP Filesystem Server for TodoList Project
# Provides enhanced file system access for AI assistants

Write-Host "=== TodoList MCP Filesystem Server ===" -ForegroundColor Cyan
Write-Host "Starting MCP Filesystem Server for TodoList C# Solution..." -ForegroundColor Green

# Check if we're in the right directory
if (-not (Test-Path "TodoApp.Api") -or -not (Test-Path "TodoApp.Web") -or -not (Test-Path "TodoApp.Shared")) {
    Write-Warning "Not in TodoList project root directory!"
    Write-Host "Please run this script from the todolist folder" -ForegroundColor Yellow
    exit 1
}

# Display project structure
Write-Host "`n=== Project Structure ===" -ForegroundColor Cyan
$projects = @("TodoApp.Api", "TodoApp.Web", "TodoApp.Shared")
foreach ($project in $projects) {
    if (Test-Path $project) {
        Write-Host "✅ $project" -ForegroundColor Green
        $csprojFile = Get-ChildItem "$project\*.csproj" -ErrorAction SilentlyContinue
        if ($csprojFile) {
            Write-Host "   📄 $($csprojFile.Name)" -ForegroundColor Gray
        }
    } else {
        Write-Host "❌ $project (missing)" -ForegroundColor Red
    }
}

Write-Host "`n🤖 MCP Filesystem Features:" -ForegroundColor Cyan
Write-Host "  • Enhanced file operations" -ForegroundColor White
Write-Host "  • Project-aware navigation" -ForegroundColor White
Write-Host "  • Smart code completion" -ForegroundColor White
Write-Host "  • Context-aware suggestions" -ForegroundColor White

Write-Host "`nStarting MCP Filesystem server..." -ForegroundColor Blue
Write-Host "Base path: $(Get-Location)" -ForegroundColor Gray
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow

# Start MCP server
try {
    $basePath = (Get-Location).Path
    Write-Host "Command: npx @modelcontextprotocol/server-filesystem $basePath" -ForegroundColor Gray
    npx @modelcontextprotocol/server-filesystem $basePath
} catch {
    Write-Error "Failed to start MCP Filesystem server: $_"
    Write-Host "`nTroubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Ensure mcp-server-filesystem is installed globally: npm install -g @modelcontextprotocol/server-filesystem" -ForegroundColor White
    Write-Host "2. Check Node.js and npm versions: node --version && npm --version" -ForegroundColor White
    Write-Host "3. Try manual test: npx -y @modelcontextprotocol/server-filesystem $basePath" -ForegroundColor White
    Write-Host "4. If npx fails, ensure Node.js is installed and PATH is correct" -ForegroundColor White
    exit 1
}