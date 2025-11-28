using System;

namespace TextSummarizer.API.Models;

public class SummarizeRequest
{public string Text { get; set; } = string.Empty;

 // "short", "medium", "long", or "custom"
    public string? Length { get; set; } = "medium";

    // Only applies if Length == "custom"
    public int? MaxLength { get; set; }

 
}
