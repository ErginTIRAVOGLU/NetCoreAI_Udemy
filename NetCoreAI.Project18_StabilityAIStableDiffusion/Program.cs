using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("🤖 Prompt'tan Görsel Üretici V1 - Stability AI");
 
string prompt = "a man wearing sunglasses on a beach";

string apiKey = "sk-Xgn5WXenouOm0CVZSJi9Ri4xXef74YvRLU9ymhekCnwOXEhJ";
string engineId = "stable-diffusion-xl-1024-v1-0";
string apiUrl = $"https://api.stability.ai/v1/generation/{engineId}/text-to-image";

using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

var requestBody = new
{
    text_prompts = new[]
    {
        new
        {
            text=prompt
        }
    },
    cfg_scale = 12,
    height = 1024,
    width = 1024,
    steps = 30,
    samples = 1
};

var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
var response = await httpClient.PostAsync(apiUrl, jsonContent);

if (!response.IsSuccessStatusCode)
{
    Console.WriteLine("Hata: " + response.StatusCode);
    var error = await response.Content.ReadAsStringAsync();
    Console.WriteLine(error);
    return;
}

var responseString = await response.Content.ReadAsStringAsync();
var responseJson = JsonDocument.Parse(responseString);

string base64Image = responseJson
    .RootElement
    .GetProperty("artifacts")[0]
    .GetProperty("base64")
    .GetString();

byte[] imageBytes = Convert.FromBase64String(base64Image);
string fileName = $"generated_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
await File.WriteAllBytesAsync(fileName, imageBytes);

Console.WriteLine($"🎈 Görsel başarıyla oluşturuldu ve kaydedildi: {fileName}");