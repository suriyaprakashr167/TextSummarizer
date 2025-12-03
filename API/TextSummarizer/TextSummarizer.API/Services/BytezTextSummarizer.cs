using System;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TextSummarizer.API.Settings;

namespace TextSummarizer.API.Services;

public class BytezTextSummarizer : ITextSummarizer
{
    private readonly HttpClient _httpClient;
    private readonly BytezSettings _settings;
    private readonly ILogger<BytezTextSummarizer> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public BytezTextSummarizer(
        HttpClient httpClient,
        IOptions<BytezSettings> settings,
        ILogger<BytezTextSummarizer> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;

        if (!string.IsNullOrWhiteSpace(_settings.ModelUrl))
        {
            _httpClient.BaseAddress = new Uri(_settings.ModelUrl);
        }

        if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", _settings.ApiKey);
        }
    }
public async Task<string> SummarizeAsync(string text, string? length = "medium", int? customMax = null)
{
    try
    {
        // Remove invalid unicode (smart quotes, emoji, control chars)
        text = new string(text.Where(c => !char.IsControl(c)).ToArray());

        // Soft truncate to avoid CUDA crash
        if (text.Length > 3000)
        {
            text = text[..3000];
        }

        // Determine target summary size
        int maxLen = 110;
        int minLen = 40;

        switch (length?.ToLower())
        {
            case "short":
                maxLen = 60;
                minLen = 20;
                break;
            case "medium":
                maxLen = 150;
                minLen = 60;
                break;
            case "long":
                maxLen = 300;
                minLen = 150;
                break;
            case "custom":
                if (customMax.HasValue)
                {
                    maxLen = customMax.Value;
                    minLen = Math.Max(10, maxLen / 3);
                }
                break;
        }

        // Hard safety bounds
        if (maxLen < 20) maxLen = 20;
        if (maxLen > 1000) maxLen = 1000;

        var payload = new { text, max_length = maxLen, min_length = minLen };

        using var response = await _httpClient.PostAsJsonAsync("", payload);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // ⏳ Retry once with safer settings
            if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                return await RetrySafely(text);
            }

            throw new Exception(responseJson);
        }

        // Expected response format: [{"summary_text": "..."}]
        var doc = JsonDocument.Parse(responseJson);

        if (doc.RootElement.ValueKind == JsonValueKind.Array &&
            doc.RootElement.GetArrayLength() > 0 &&
            doc.RootElement[0].TryGetProperty("summary_text", out var result))
        {
            return result.GetString()?.Trim('"').Replace("\\\"", "\"") ?? "No summary.";
        }

        // Try format 2: {"error": null, "output": "..."}
        if (doc.RootElement.ValueKind == JsonValueKind.Object &&
            doc.RootElement.TryGetProperty("output", out var outputProp))
        {
            var output = outputProp.GetString();
            if (!string.IsNullOrWhiteSpace(output))
            {
                return output.Trim();
            }
        }

        // If error field exists and has a value, include it in the response
        if (doc.RootElement.ValueKind == JsonValueKind.Object &&
            doc.RootElement.TryGetProperty("error", out var errorProp) &&
            errorProp.ValueKind != JsonValueKind.Null)
        {
            var errorMsg = errorProp.GetString();
            if (!string.IsNullOrWhiteSpace(errorMsg))
            {
                return $"❌ API Error: {errorMsg}";
            }
        }

        return "❌ Unexpected response format from API.";
    }
    catch (Exception ex)
    {
        return $"❌ Error: {ex.Message}";
    }
}

private async Task<string> RetrySafely(string text)
{
    var fallbackPayload = new { text, max_length = 80 };

    using var retryResponse = await _httpClient.PostAsJsonAsync("", fallbackPayload);
    var retryJson = await retryResponse.Content.ReadAsStringAsync();

    if (!retryResponse.IsSuccessStatusCode)
        return $"❌ Bytez failed twice.\nResponse:\n{retryJson}";

    var doc = JsonDocument.Parse(retryJson);

    if (doc.RootElement.ValueKind == JsonValueKind.Array &&
        doc.RootElement.GetArrayLength() > 0 &&
        doc.RootElement[0].TryGetProperty("summary_text", out var result))
    {
        return result.GetString()!;
    }

    // Try format 2: {"error": null, "output": "..."}
    if (doc.RootElement.ValueKind == JsonValueKind.Object &&
        doc.RootElement.TryGetProperty("output", out var outputProp))
    {
        var output = outputProp.GetString();
        if (!string.IsNullOrWhiteSpace(output))
        {
            return output.Trim();
        }
    }

    return "❌ Unexpected response format from API.";
}


}