using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

string apiKey = "AIzaSyCE-bu0d6mdLRG5Yd3lxA0UnA56fKPb65g";
string model = "gemini-2.5-pro";
string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

string question = "What is the capital of Turkey?";

var requestBody = new
{
    contents = new[]
    {
        new
        {
            parts = new[]
            {
                new
                {
                    text = question
                }
            }
        }
    }
};

var json = JsonSerializer.Serialize(requestBody);
var content = new StringContent(json, Encoding.UTF8, "application/json");

using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

var response = await httpClient.PostAsync(endpoint, content);
var responseContent = await response.Content.ReadAsStringAsync();

try
{
    var doc = JsonDocument.Parse(responseContent);
    var root = doc.RootElement;

    var answer = root
        .GetProperty("candidates")[0]
        .GetProperty("content")
        .GetProperty("parts")[0]
        .GetProperty("text")
        .GetString();

    Console.WriteLine($"Question: {question}");
    Console.WriteLine($"Answer: {answer}");
}
catch (JsonException)
{
    Console.WriteLine("Error parsing JSON response:");
    Console.WriteLine(responseContent);
}