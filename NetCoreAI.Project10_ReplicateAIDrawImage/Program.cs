

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

string apiKey = "r8_WLsfIJEQPcQoICGSy2NtxOz3FfqOQiq2qgvop";
string apiUrl = "https://api.replicate.com/v1/predictions";

string prompt = "A futuristic cityscape at sunset, digital art";

var requestBody=new
{
    version = "7762fd07cf82c948538e41f63f77d685e02b063e37e496e96eefd46c929f9bdc",  
    input = new
    {
        prompt = prompt       
    }
};

var json = JsonSerializer.Serialize(requestBody);
using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", apiKey);
httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

var content = new StringContent(json, Encoding.UTF8, "application/json");

Console.WriteLine("Sending request to Replicate API...");
var response = await httpClient.PostAsync(apiUrl, content);
var responseContent = await response.Content.ReadAsStringAsync();

Console.WriteLine("Response from Replicate API:");
Console.WriteLine(responseContent);