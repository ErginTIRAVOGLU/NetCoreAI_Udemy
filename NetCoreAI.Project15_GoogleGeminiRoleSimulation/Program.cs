using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

string apiKey = "AIzaSyCE-bu0d6mdLRG5Yd3lxA0UnA56fKPb65g";
string model = "gemini-2.5-pro";
string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

Console.WriteLine("Rolünüzü Seçin: ");
Console.WriteLine("1- Psikolog");
Console.WriteLine("2- Maç Yorumcusu");
Console.WriteLine("3- Finansal Yatırım Uzmanı");
Console.WriteLine("4- Tarihçi");
Console.WriteLine("5- Turist Rehberi");
Console.WriteLine();
Console.Write("Seçiminiz: ");
string roleChoice = Console.ReadLine();

string rolePrompt = roleChoice switch
{
    "1" => "Sen bir psikologsun. Danışanının sorularına empatik, açıklayıcı ve sakin bir dille yanıt ver",
    "2" => "Sen bir maç yorumcususun. Sorulan soruya sanki maç başlamadan birkaç saat önce stada gitmiş gibi o atmosferi hisseden ve heyencanlandıran cevap ver.",
    "3" => "Sen bir finans yatırım danışmanısın. Kullanıcının bütçesine ve hedeflerine göre yatırım önerileri yap.",
    "4" => "Sen bir tarihçisin. Olayları bilimsel kaynaklara dayanarak detaylı olarak anlat.",
    "5" => "Sen bir turist rehberisin, sorulan soruya o şehri çok iyi bilen, o şehirde mutlaka görülmesi gereken ve yenilmesi gereken lezzetleri listeleyek yanıt ver"
};

Console.WriteLine();

Console.Write("Sormak istediğiniz cümleyi giriniz: ");
string userInput = Console.ReadLine();
string finalPrompt = $"{rolePrompt}\n\n Kullanıcıdan Gelen Soru: {userInput}";

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
                            text=finalPrompt
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
    string answer = doc.RootElement
        .GetProperty("candidates")[0]
        .GetProperty("content")
        .GetProperty("parts")[0]
        .GetProperty("text")
        .GetString();

    Console.WriteLine(answer);
}
catch
{
    Console.WriteLine("Yanıt hatası: " + responseText);
}