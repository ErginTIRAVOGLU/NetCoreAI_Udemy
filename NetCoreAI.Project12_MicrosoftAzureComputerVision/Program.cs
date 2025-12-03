using System.Net.Http.Headers;
using System.Text.Json;

string imagePath = "./assets/04.jpg";
string subscriptionKey = "8KDqv9yXnm5PFtshgLQ2EuwcBZp6YxRrN3Wj0FzTAkV4bH1MdS7CilOQQJ99BFAC5RqLJXJ3w3AAAFAGHMYtNcP";
string endpoint = "https://ergin-vision-ai.cognitiveservices.azure.com";

string apiUrl = $"{endpoint}/vision/v3.2/analyze";

string requestParameters = "visualFeatures=Categories,Description,Tags,Color&language=en";
string uri = apiUrl + "?" + requestParameters;

if (!File.Exists(imagePath))
{
    Console.WriteLine("Görsel dosyası bulunamadı!" + imagePath);
    return;
}

byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);

using (HttpClient client = new HttpClient())
using (ByteArrayContent content = new ByteArrayContent(imageBytes))
{
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

    HttpResponseMessage response = await client.PostAsync(uri, content);
    string result = await response.Content.ReadAsStringAsync();

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("Azure Yanıtı: ");
        JsonDocument json = JsonDocument.Parse(result);
        var description = json.RootElement.GetProperty("description").GetProperty("captions")[0];
        string text = description.GetProperty("text").GetString();
        double confidence = description.GetProperty("confidence").GetDouble();

        Console.WriteLine($"Açıklama: {text} (Güven: %{confidence * 100:0.00})");
    }
    else
    {
        Console.WriteLine("bir hata oluştu!");
        Console.WriteLine($"Status {response.StatusCode}");
        Console.WriteLine("Yanıt: " + result);
    }
}