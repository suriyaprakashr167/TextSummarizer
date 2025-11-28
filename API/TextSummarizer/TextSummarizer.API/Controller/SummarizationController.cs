using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TextSummarizer.API.Models;
using TextSummarizer.API.Services;

namespace TextSummarizer.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummarizationController : ControllerBase
    {
    private readonly ITextSummarizer _summarizer;
    private readonly ILogger<SummarizationController> _logger;

    public SummarizationController(
        ITextSummarizer summarizer,
        ILogger<SummarizationController> logger)
    {
        _summarizer = summarizer;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<SummarizeResponse>> Summarize([FromBody] SummarizeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Text is required.");

        try
        {
            var summary = await _summarizer.SummarizeAsync(
    request.Text,
    request.Length,
    request.MaxLength
);


            var response = new SummarizeResponse
            {
                Summary = summary
            };

            return Ok(response);
        }
        catch (Exception ex)
{
    _logger.LogError(ex, "Failed to summarize text");
    return StatusCode(500, ex.Message + "\n\n" + ex.InnerException?.Message);
}
    }
}
}
