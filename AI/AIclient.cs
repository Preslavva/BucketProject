using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using BucketProject.Infrastructure.AI;
using BucketProjetc.BLL.Business_Logic.InterfacesService;
using Microsoft.Extensions.Configuration;
using BucketProject.DAL.Models.Enums;
public class AIClient : IAIClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AIClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<List<string>> BreakDownTextIntoGoalsAsync(string description, Category category)
    {
        string prompt = $@"Break down the following goal into 4 smaller, actionable sub-goals:
""{description}""
Take into consideration that this goal is meant to be accomplished within a {category.ToString().ToLower()} timeframe.
Write each sub-goal as a plain sentence.    
Do not number them, do not use bullets, dashes, or any formatting.
Return exactly four plain sentences, each separated by a line break.
Ensure the sub-goals are unique and tailored to the main goal every time you generate them";


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
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"OpenAI API call failed. Status: {(int)response.StatusCode} - {response.ReasonPhrase}. Response body: {errorContent}");
        }


        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ChatResponse>(json);

        string reply = result?.Choices?[0]?.Message?.Content;
        if (string.IsNullOrWhiteSpace(reply))
            throw new Exception("OpenAI response is empty.");

        return reply.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();


    }
}
