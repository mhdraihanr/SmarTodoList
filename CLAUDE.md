# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Common Commands

- Build the solution: `dotnet build`
- Run the API: `dotnet run --project TodoApp.Api`
- Run the Web app: `dotnet run --project TodoApp.Web`
- Run tests: `dotnet test`
- Run a single test: `dotnet test --filter &quot;TestMethodName&quot;`
- Format code: `dotnet format`

## Architecture Overview

This is a TodoList application built with .NET. The solution includes:
- **TodoApp.Api**: ASP.NET Core Web API for backend, containing Controllers for endpoints, Services for business logic, and Models for data.
- **TodoApp.Shared**: Shared project with DTOs (e.g., TodoItemDto) used across API and Web.
- **TodoApp.Web**: Blazor WebAssembly frontend, with Pages for UI components, Services for client-side logic, and wwwroot for static assets.

The Blazor app consumes the API for todo operations. Integration with Model Context Protocol (MCP) for GitHub and filesystem operations is configured via mcp.json and related scripts.