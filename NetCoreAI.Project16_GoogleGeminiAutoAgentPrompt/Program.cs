
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

string apiKey = "AIzaSyCE-bu0d6mdLRG5Yd3lxA0UnA56fKPb65g";
string model = "gemini-2.5-pro";
string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

string context = "Sen bir yapay zeka içerik planlayıcısısın. Kullanıcının fikrine göre içerik üretmesine yardım edeceksin. Fikri aldıktan sonra kullanıcıya doğru soruları sorarak onu yönlendirecek ve sonunda içerik planını çıkaracaksın.";

 

string userInput = "Bir kafe açmak istiyorum";

string userPrompt = $"{context}\n\n Kullanıcının fikri: {userInput}\n Şimdi ona adım adım sorular sormaya başla.";


for (int i = 0; i < 5; i++)
{
    string question = await SendToGemini(apiKey, endpoint, userPrompt);
    Console.WriteLine($"Agent: {question}");

    Console.Write("Sen: ");
    string answer = Console.ReadLine();

    userPrompt += $"\n\nKullanıcının Cevabı: {answer}\n Yeni sorunu sor.";
}

string finalPrompt = $"{userPrompt}\n\n Artık yeterli bilgiye sahipsin. Kullanıcı için detaylı bir içerik planı oluştur";
string finalOutput = await SendToGemini(apiKey, endpoint, finalPrompt);

Console.WriteLine("\n Nihai İçerik Planı: \n" + finalOutput);

static async Task<string> SendToGemini(string apiKey, string endpoint, string prompt)
    {
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
                            text=prompt
                        }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.PostAsync(endpoint, content);
        var responseText = await response.Content.ReadAsStringAsync();

        try
        {
            var doc = JsonDocument.Parse(responseText);
            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
        }
        catch
        {
            return "Bir hata oluştu!";
        }
    }