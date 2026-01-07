using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Domain.Services;

public interface IGeminiApiClient
{
    Task<string> SendRequestAsync(string prompt, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> SendStreamingRequestAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default);
}

public class GeminiApiClient : IGeminiApiClient
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiApiClient> _logger;
    private const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp";

    public GeminiApiClient(
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<GeminiApiClient> logger)
    {
        _apiKey = configuration["AiSettings:GoogleApiKey"]!;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> SendRequestAsync(string prompt, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Sending non-streaming request to Gemini API");
        
        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                response_mime_type = "application/json"
            }
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(
            $"{BASE_URL}:generateContent?key={_apiKey}",
            jsonContent,
            cancellationToken);

        _logger.LogDebug("Gemini API Status: {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Gemini API Error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
            return "AI service is not available at the moment.";
        }

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        
        try
        {
            using var doc = JsonDocument.Parse(responseString);
            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gemini Parsing Error. Raw Response: {RawResponse}", responseString);
            return "AI Error";
        }
    }

    public async IAsyncEnumerable<string> SendStreamingRequestAsync(
        string prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Sending streaming request to Gemini API");

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
            // NOTE: No response_mime_type for streaming - we want plain text chunks!
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{BASE_URL}:streamGenerateContent?alt=sse&key={_apiKey}")
        {
            Content = jsonContent
        };

        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Gemini Streaming Error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
            yield return "Error: AI service unavailable";
            yield break;
        }

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(line)) continue;

            // SSE format: "data: {json}"
            if (line.StartsWith("data: "))
            {
                var jsonData = line.Substring(6); // Remove "data: " prefix

                // Extract text from candidates[0].content.parts[0].text
                using var doc = JsonDocument.Parse(jsonData);
                
                if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0)
                {
                    var candidate = candidates[0];
                    if (candidate.TryGetProperty("content", out var content) &&
                        content.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0)
                    {
                        var part = parts[0];
                        if (part.TryGetProperty("text", out var textElement))
                        {
                            var chunk = textElement.GetString();
                            if (!string.IsNullOrEmpty(chunk))
                            {
                                yield return chunk;
                            }
                        }
                    }
                }
            }
        }
    }
}
