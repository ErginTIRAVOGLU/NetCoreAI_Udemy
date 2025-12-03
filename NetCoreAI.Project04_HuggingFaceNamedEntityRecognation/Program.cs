using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

var apikey = "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";
var inputText = "During his visit to Germany last September, Apple CEO Tim Cook met with executives from Siemens in Munich to discuss potential collaborations in renewable energy technologies.";

using (var httpClient = new HttpClient())
{
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);

    var requestBody = new
    {
        inputs = inputText
    };

    var json = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var response = await httpClient.PostAsync("https://router.huggingface.co/hf-inference/models/dslim/bert-base-NER", content);
    var responseString = await response.Content.ReadAsStringAsync();

    Console.WriteLine("Named Entity Recognition Sonucu:");
    Console.WriteLine();

    var doc = JsonDocument.Parse(responseString);
    foreach (var element in doc.RootElement.EnumerateArray())
    {
        var entity = element.GetProperty("entity_group").GetString();
        var word = element.GetProperty("word").GetString();
        var score = Math.Round(element.GetProperty("score").GetDouble() * 100, 2);

        Console.WriteLine($"Kelime: {word}");
        Console.WriteLine($".    |-Türü: {entity}");
        Console.WriteLine($".    |-Score: {score}%");
    }

}
