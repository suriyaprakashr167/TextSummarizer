using System;

namespace TextSummarizer.API.Services;

public interface ITextSummarizer
{
Task<string> SummarizeAsync(string text, string? length = "medium", int? customMax = null);
}
