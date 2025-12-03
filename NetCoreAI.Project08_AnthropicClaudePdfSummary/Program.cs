using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;

string pdfPath = "./assets/kitap.pdf";
string apiKey = "sk-ant-api03-B9Qm7K44TxpLSHJeDPKz8WdlfR2Nv5b4X6pYZguHT33rQPAKd1L9BME7sQ_fhTQ8ZgVr9HK72-4NNXGsQp821D-RtKxwAA";

if (!File.Exists(pdfPath))
{
    Console.WriteLine("Pdf Dosyası bulunamadı!");
    return;
}

string pdfText = "";
using (var document = PdfDocument.Open(pdfPath))
{
    foreach (var page in document.GetPages())
    {
        pdfText += page.Text + "\n";
    }
}

string prompt = $"Aşağıdaki metni detaylıca özetle.\n\n{pdfText}";

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
                    role = "user",
                    content = prompt
                }
            }
};

var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

var response = await client.PostAsync("v1/messages", jsonContent);
var reponseString = await response.Content.ReadAsStringAsync();

Console.WriteLine("Claude Pdf Özeti: ");
Console.WriteLine(reponseString);
