using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using BucketProject.Infrastructure.AI;

public class OpenAIClient : IAIClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public OpenAIClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<List<string>> BreakDownTextIntoGoalsAsync(string description)
    {
        var prompt = $"Break down the following goal into 4 smaller, actionable sub-goals:\n\"{description}\"";

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _configuration["OpenAI:ApiKey"]);

        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[] { new { role = "user", content = prompt } }
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("OpenAI API call failed.");

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ChatResponse>(json);

        string reply = result?.choices?[0]?.message?.content;
        if (string.IsNullOrWhiteSpace(reply))
            throw new Exception("OpenAI response is empty.");

        return Regex.Split(reply.Trim(), @"\n\d+\.\s*")
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
    }
}
