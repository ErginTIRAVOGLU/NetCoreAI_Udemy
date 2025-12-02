using System.Net.Http.Headers;
using System.Text.Json;

var apiKey="7aa21f092ba058e093e889b7ee6fdfb8421f58cf";
var filePath="./files/testeng.mp3";
var filePathTr="./files/testtr.mp3";

if(!File.Exists(filePathTr))
{
    Console.WriteLine($"File not found: {filePathTr}");
    return;
}

using var httpClient=new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", apiKey);

using var fileStream=File.OpenRead(filePathTr);

var content = new StreamContent(fileStream);
content.Headers.ContentType = new MediaTypeHeaderValue("audio/mp3");

var response = await httpClient.PostAsync("https://api.deepgram.com/v1/listen?punctuate=true&language=tr", content);
var json = await response.Content.ReadAsStringAsync();

try
{
    var doc = JsonDocument.Parse(json);
    var transcript = doc.RootElement
        .GetProperty("results")
        .GetProperty("channels")[0]
        .GetProperty("alternatives")[0]
        .GetProperty("transcript")
        .GetString();
    Console.WriteLine("");
    Console.WriteLine("Transcription:");
    Console.WriteLine(transcript);
}
catch (Exception ex)
{
    Console.WriteLine("Failed to parse JSON response:");
    Console.WriteLine(ex.Message);
    Console.WriteLine("Response content: \n" + json);
    return;
}