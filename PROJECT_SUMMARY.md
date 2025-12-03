# TextSummarizer API - Project Summary

## Overview

**TextSummarizer** is an ASP.NET Core Web API that provides text summarization capabilities. It integrates with the Bytez AI model API to generate concise summaries of longer texts with configurable length options.

---

## Project Structure

```
TextSummarizer/
├── TextSummarizer.API/
│   ├── Program.cs                          # Application entry point & dependency injection
│   ├── TextSummarizer.API.csproj          # Project file with NuGet dependencies
│   ├── appsettings.json                   # Application configuration
│   ├── appsettings.Development.json       # Development-specific settings
│   ├── Properties/
│   │   └── launchSettings.json            # Launch profiles (HTTP/HTTPS)
│   ├── Controller/
│   │   └── SummarizationController.cs     # API endpoint for text summarization
│   ├── Models/
│   │   ├── SummarizeRequest.cs            # Request DTO
│   │   └── SummarizeResponse.cs           # Response DTO
│   ├── Services/
│   │   ├── ITextSummarizer.cs             # Service interface
│   │   └── BytezTextSummarizer.cs         # Implementation using Bytez API
│   └── Settings/
│       └── BytezSettings.cs               # Configuration settings class
└── TextSummarizer.sln                     # Visual Studio solution file
```

---

## Technology Stack

| Component | Version |
|-----------|---------|
| .NET | 10.0 |
| ASP.NET Core | Web API |
| Newtonsoft.Json | 13.0.4 |
| RestSharp | 112.1.0 |
| Swashbuckle.AspNetCore (Swagger) | 10.0.1 |
| Microsoft.AspNetCore.OpenApi | 10.0.0-rc.1.25451.107 |

---

## Key Features

### 1. **Text Summarization API**
- **Endpoint**: `POST /api/summarization`
- **Purpose**: Accepts text input and returns a summarized version

### 2. **Flexible Length Options**
The API supports predefined length modes:
- **short**: ~50 characters
- **medium**: ~110 characters (default)
- **long**: ~200 characters
- **custom**: User-defined max length

### 3. **External AI Integration**
- Integrates with Bytez AI model API
- Configurable via settings (API key and model URL)
- Includes error handling and logging

### 4. **Swagger/OpenAPI Documentation**
- Automatic API documentation available at `/swagger`
- Interactive endpoint testing

---

## Core Components

### **Models**

#### SummarizeRequest
```csharp
public class SummarizeRequest
{
    public string Text { get; set; }        // Required: Text to summarize
    public string? Length { get; set; }     // Optional: "short", "medium", "long", "custom"
    public int? MaxLength { get; set; }     // Optional: Only for "custom" length
}
```

#### SummarizeResponse
```csharp
public class SummarizeResponse
{
    public string Summary { get; set; }     // The summarized text
}
```

### **Services**

#### ITextSummarizer Interface
```csharp
public interface ITextSummarizer
{
    Task<string> SummarizeAsync(string text, string? length = "medium", int? customMax = null);
}
```

#### BytezTextSummarizer Implementation
- Communicates with Bytez AI model
- Handles HTTP requests/responses with configurable base address
- Supports API authentication via headers
- Parses JSON responses from Bytez API
- Includes fallback property names (`summary_text` or `generated_text`)
- Error handling with detailed logging

### **Controller**

#### SummarizationController
- Route: `/api/summarization`
- HTTP Method: POST
- Validation: Ensures text is not empty
- Exception Handling: Returns 500 status with error details
- Dependency Injection: Uses ITextSummarizer and ILogger

### **Settings**

#### BytezSettings
- `ApiKey`: Authorization token for Bytez API
- `ModelUrl`: Base URL for the Bytez model endpoint
- Configured via `appsettings.json` under "Bytez" section

---

## Configuration

### Application Settings (`appsettings.json`)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Launch Profiles
- **HTTP**: `http://localhost:5104`
- **HTTPS**: `https://localhost:7008` (with HTTP fallback to 5104)
- Environment: Development by default

---

## Dependency Injection Setup

The `Program.cs` configures:
1. **HttpClient**: For Bytez API communication
2. **ITextSummarizer**: Registered with BytezTextSummarizer implementation
3. **BytezSettings**: Bound from configuration
4. **Controllers**: ASP.NET Core MVC
5. **Swagger**: API documentation and testing UI

---

## API Usage Example

### Request
```http
POST /api/summarization
Content-Type: application/json

{
  "text": "Your long text to summarize here...",
  "length": "medium",
  "maxLength": null
}
```

### Response (Success - 200 OK)
```json
{
  "summary": "Concise summary of the provided text..."
}
```

### Response (Error - 400 Bad Request)
```json
{
  "error": "Text is required."
}
```

### Response (Error - 500 Internal Server Error)
```json
{
  "error": "Bytez API Error → Status: 500\nResponse: {...}"
}
```

---

## Error Handling

1. **Validation**: Empty or null text input rejected with 400 status
2. **API Errors**: Bytez API failures caught and returned with 500 status
3. **Parsing Errors**: JSON parsing issues handled with fallback responses
4. **Logging**: All exceptions logged via `ILogger<SummarizationController>`

---

## Future Enhancements

- [ ] Add caching for repeated summarization requests
- [ ] Support for multiple summarization models/providers
- [ ] Rate limiting and quota management
- [ ] Batch summarization endpoint
- [ ] Request/response logging and analytics
- [ ] Authentication/authorization layer
- [ ] Unit and integration tests

---

## Development Workflow

### Prerequisites
- .NET 10.0 SDK
- Visual Studio or VS Code with C# extension

### Running the Application
```bash
dotnet run --project TextSummarizer.API/TextSummarizer.API.csproj
```

### Testing Endpoints
- Access Swagger UI: `http://localhost:5104/swagger`
- Use HTTP client files or Postman for API testing

### Building
```bash
dotnet build TextSummarizer.sln
```

---

## Repository Information

- **Repository Name**: TextSummarizer
- **Owner**: suriyaprakashr167
- **Current Branch**: suriyadev
- **Location**: `d:\textsummarizerGit\API\TextSummarizer`

---

## File Descriptions

| File | Purpose |
|------|---------|
| `Program.cs` | Application startup, DI configuration, middleware setup |
| `SummarizationController.cs` | HTTP endpoint handler for summarization requests |
| `SummarizeRequest.cs` | Input model for summarization requests |
| `SummarizeResponse.cs` | Output model for summarization responses |
| `ITextSummarizer.cs` | Service contract/interface |
| `BytezTextSummarizer.cs` | Implementation of text summarization via Bytez API |
| `BytezSettings.cs` | Configuration model for API credentials |
| `appsettings.json` | Application configuration (logging, hosts) |
| `launchSettings.json` | Launch profiles for HTTP/HTTPS |
| `TextSummarizer.API.csproj` | Project configuration and dependencies |

---

## Notes

- The API is built on ASP.NET Core 10.0 (latest version)
- All properties use nullable reference types (`#nullable enable`)
- HttpClient is configured with dependency injection for testability
- The Bytez integration expects JSON array responses with either `summary_text` or `generated_text` properties
- Default summarization length is "medium" (~110 characters)
