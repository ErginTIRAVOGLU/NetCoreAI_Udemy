
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

var apikey = "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";

//var commentText = "I really don’t agree with your opinion, but I appreciate that you explained it clearly."; 
//var commentText = "Your attitude is really annoying, and you keep talking like you know everything."; 
var commentText = "You're completely useless and no one wants to hear anything you say."; 
 

using (var client = new HttpClient())
{
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", apikey);

    var requestBody= new
    {
        inputs =  commentText
    };

    var json = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(json,Encoding.UTF8,"application/json");
    var response = await client.PostAsync("https://router.huggingface.co/hf-inference/models/unitary/toxic-bert", content);

    var responseString =  await response.Content.ReadAsStringAsync();
    if(!responseString.StartsWith("["))
    {
        Console.WriteLine("Unexpected response format:");
        Console.WriteLine(responseString);
        return;
    }
    var doc = JsonDocument.Parse(responseString);
 
    Console.WriteLine("Yorum sonucu:");
    foreach (var element in doc.RootElement[0].EnumerateArray())
    {
        var label = element.GetProperty("label").GetString();
        var score = Math.Round(element.GetProperty("score").GetDouble() * 100, 2);
        Console.WriteLine($".    |- {label}: {score}%");
    }

    
    
}


