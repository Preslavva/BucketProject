using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using BucketProject.Infrastructure.AI;
using BucketProject.BLL.Business_Logic.InterfacesService;
using Microsoft.Extensions.Configuration;
using BucketProject.DAL.Models.Enums;
using Exceptions.Exceptions;
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
        string prompt = $@"
Break down the following goal into 4 smaller, actionable sub-goals:

""{description}""

This goal is intended to be completed within a {category.ToString().ToLower()} timeframe.

Each sub-goal should be written as a plain sentence.  
Do not number them or use bullets, dashes, or any formatting.  
Do not mention specific days, weeks, dates, months, or times.  
Return exactly four plain sentences, each on a new line.  
Make sure the sub-goals are unique, clearly related to the main goal, and not repetitive.  
Each sub-goal must be under 50 characters, including spaces.
Each sub-goal must be measurable: phrase it so its completion can be verified with a clear ‘done/not-done’ outcome, allowing a specific completion date to be assigned (no date should appear in the text).
Each sub-goal must describe a concrete action, not an instruction to set or track another goal.";


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

            string userMessage = "We couldn't generate sub-goals right now. Please try again later.";
            string devMessage = $"OpenAI API call failed. Status: {(int)response.StatusCode} - {response.ReasonPhrase}. Response body: {errorContent}";

            throw new AIRequestFailedException(userMessage, devMessage);

        }


        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ChatResponse>(json);

        string reply = result?.Choices?[0]?.Message?.Content;
        if (string.IsNullOrWhiteSpace(reply))
            throw new EmptyAIResponseException();

        return reply.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
    }
}
