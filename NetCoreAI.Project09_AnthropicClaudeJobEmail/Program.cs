using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

string apiKey = "sk-ant-api03-B9Qm7K44TxpLSHJeDPKz8WdlfR2Nv5b4X6pYZguHT33rQPAKd1L9BME7sQ_fhTQ8ZgVr9HK72-4NNXGsQp821D-RtKxwAA";

string prompt = @"Bana 'Yazılım Geliştirici' pozisyonu için hazırlanan, profesyonel ama samimi tonda bir iş başvuru e-postası yazar mısın? 
    Adım Murat, 5 yıldır .Net geliştiricisiyim, ekip çalışmasına yatkınım, ve uzaktan çalışmaya uygunum.";

using var client = new HttpClient();
client.BaseAddress = new Uri("https://api.anthropic.com/");
client.DefaultRequestHeaders.Add("x-api-key", apiKey);
client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

var requestBody = new
{
    model = "claude-3-opus-20240229",
    max_tokens = 1000,
    temperature = 0.5,
    messages = new[]
    {
                new
                {
                    role="user",
                    content=prompt
                }
            }
};

var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
var response = await client.PostAsync("v1/messages", jsonContent);
var responseString = await response.Content.ReadAsStringAsync();

var json = JsonNode.Parse(responseString);
string? textContent = json?["content"]?[0]?["text"]?.ToString();

if(textContent == null)
{
    Console.WriteLine("E-Posta oluşturulamadı.");
    Console.WriteLine(responseString);
    return;
}
Console.WriteLine("Oluşturulan E-Posta");
Console.WriteLine(textContent);