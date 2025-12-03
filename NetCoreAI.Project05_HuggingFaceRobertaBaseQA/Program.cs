
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

var apikey = "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";

var contextText = "In 2023, the European Space Agency (ESA) launched a new climate observation satellite named Aurora, designed to monitor atmospheric changes with unprecedented precision. The satellite was developed in collaboration with researchers from the University of Cambridge and several technology companies across Europe. Aurora’s primary mission is to track long-term greenhouse gas emissions and provide data to help governments create more effective climate policies. The satellite is expected to operate for at least ten years, sending daily environmental reports to ESA’s control center in Paris.";
//var questionText = "What is the name of the climate observation satellite launched by ESA?";
//var questionText = "Which university contributed to the development of the satellite?";
//var questionText = "Where will the daily environmental reports be sent?"; 
var questionText = "What was the total cost of developing the Aurora satellite?";
using (var client = new HttpClient())
{
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", apikey);

    var requestBody= new
    {
        inputs = new
        {
            question = questionText,
            context = contextText
        }
    };

    var json = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(json,Encoding.UTF8,"application/json");
    var response = await client.PostAsync("https://router.huggingface.co/hf-inference/models/deepset/roberta-base-squad2", content);

    var responseString =  await response.Content.ReadAsStringAsync();
    var doc = JsonDocument.Parse(responseString);

    if(doc.RootElement.TryGetProperty("answer", out var answerElement))
    {
        doc.RootElement.TryGetProperty("score", out var scoreElement);
        var score = Math.Round(scoreElement.GetDouble() * 100);
        if(score > 25)
        {
            Console.WriteLine($"Question: {questionText}");
            Console.WriteLine($"Context: {contextText}");
            Console.WriteLine($"Answer: {answerElement.GetString()}");
            
        }
        else
        {
            Console.WriteLine("The model is not confident enough to provide an answer.");
        }
       Console.WriteLine($".    |- Score: {score}%");

    }
    else
    {
        Console.WriteLine("No answer found in the response.");
        Console.WriteLine(responseString);
    }
    
}


