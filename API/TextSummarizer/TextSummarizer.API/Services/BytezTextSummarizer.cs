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
    try{
    int maxLen;

    // determine length behavior
    length = length?.ToLower() ?? "medium";

    if (length == "custom" && customMax.HasValue)
        maxLen = customMax.Value;
    else
        maxLen = length switch
        {
            "short" => 50,
            "medium" => 110,
            "long" => 200,
            _ => 110
        };

    var payload = new
    {
        text = text,
        max_length = maxLen   // <-- Bytez understands this parameter
    };

    using var response = await _httpClient.PostAsJsonAsync("", payload);
    var responseJson = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        throw new Exception($"Bytez API Error → Status: {response.StatusCode}\nResponse: {responseJson}");

    // Parse
    var doc = JsonDocument.Parse(responseJson);

    if (doc.RootElement.ValueKind == JsonValueKind.Array &&
        doc.RootElement.GetArrayLength() > 0)
    {
        var item = doc.RootElement[0];

        if (item.TryGetProperty("summary_text", out var s1))
            return s1.GetString()!;

        if (item.TryGetProperty("generated_text", out var s2))
            return s2.GetString()!;
    }

    return responseJson;


}  
    catch (Exception ex)
    {
        return $"Parsing error: {ex.Message}\nRaw Response:\n";
    }


}}