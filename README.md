# SmarTodoList

Aplikasi Todo List dengan fitur AI chat menggunakan ASP.NET Core dan Blazor WebAssembly.

## Setup Konfigurasi

### 1. Konfigurasi API Key

Aplikasi ini menggunakan OpenRouter AI API. Anda perlu mendapatkan API key dari [OpenRouter](https://openrouter.ai/keys).

#### Opsi 1: Menggunakan User Secrets (Recommended untuk Development)

```bash
cd TodoApp.Api
dotnet user-secrets set "AiChat:ApiKey" "your-openrouter-api-key-here"
```

#### Opsi 2: Menggunakan Environment Variables

```bash
# Copy .env.example ke .env
cp .env.example .env
# Edit .env dan masukkan API key Anda
```

#### Opsi 3: Menggunakan appsettings.json (Tidak Recommended)

1. Copy file `appsettings.Example.json` menjadi `appsettings.json`
2. Copy file `appsettings.Development.Example.json` menjadi `appsettings.Development.json`
3. Ganti `your-openrouter-api-key-here` dengan API key Anda

**⚠️ PENTING: Jangan commit file `appsettings.json` yang berisi API key ke Git repository!**

### 2. Menjalankan Aplikasi

#### Start API:

```bash
cd TodoApp.Api
dotnet run
```

API akan berjalan di: `http://localhost:5191`

#### Start Web App:

```bash
cd TodoApp.Web
dotnet run
```

Web app akan berjalan di: `http://localhost:5251`

## Struktur Project

- `TodoApp.Api/` - Backend API dengan endpoints untuk todo dan AI chat
- `TodoApp.Web/` - Frontend Blazor WebAssembly
- `TodoApp.Shared/` - Shared models dan DTOs

## Fitur

- ✅ CRUD Todo Items
- ✅ AI Chat untuk mendapatkan saran task
- ✅ Blazor WebAssembly frontend
- ✅ ASP.NET Core minimal API backend
- ✅ User Secrets untuk API key management
- ✅ Configuration dengan Options pattern

## Keamanan

- API keys disimpan menggunakan User Secrets atau environment variables
- File konfigurasi sensitif tidak di-commit ke repository
- CORS dikonfigurasi untuk development dan production
