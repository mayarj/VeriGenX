using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using VeriGenX.Domain.Repository;


namespace VeriGenX.AIIntegration
{
    public class CodeGenerationRepository : ICodeGenerationRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiBaseUrl;
        public CodeGenerationRepository(HttpClient httpClient, string apiKey, string apiBaseUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _apiBaseUrl = apiBaseUrl ?? throw new ArgumentNullException(nameof(apiBaseUrl));

            // Configure HttpClient
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<string> GenerateTextAsync(string prompt)
        {

            return await GenerateTextAsync(prompt, 100, 0.7f);
        }

        public async Task<string > GenerateTextAsync( string prompt, int maxTokens, float temperature)
        {
            var requestBody = new
            {
                prompt = prompt,
                max_tokens = maxTokens,
                temperature = temperature
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/generate", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseJson);

            return jsonDoc.RootElement.GetProperty("text").GetString() ?? string.Empty;
        }
    }
}
