using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

var apiKey ="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";

var inputText = @"Istanbul is one of the most remarkable cities in the world, with its thousands-year-old history, unique location connecting two continents, and rich cultural diversity. Having served as the capital of three great empires—Roman, Byzantine, and Ottoman—this city offers visitors the beauty of both the ancient and modern worlds at the same time. Encountering traces of different civilizations at every step makes Istanbul not just a city but an open-air museum that must be experienced. In this article, we explore in detail the must-see spots in Istanbul, their significance, and the atmosphere they offer.

1. Hagia Sophia: A Legacy That Defies Centuries

One of the first structures that comes to mind when Istanbul is mentioned is undoubtedly Hagia Sophia. Built in 537 by the Byzantine Emperor Justinian, Hagia Sophia is among the most impressive architectural works in the world thanks to its design and captivating atmosphere. Having served as a church, a mosque, and a museum throughout its long history, Hagia Sophia stands as one of the finest representations of the intersection between religions and civilizations. Its massive dome, golden mosaics, and the transformations it has undergone over the centuries offer visitors a true journey through time. It is unimaginable to visit Istanbul and leave without witnessing the magnificence of Hagia Sophia.";
var requestData = new
{
    inputs= inputText
};

var json = JsonSerializer.Serialize(requestData);
var content = new StringContent(json, Encoding.UTF8,"application/json");

using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

//var response = await httpClient.PostAsync("https://router.huggingface.co/hf-inference/models/facebook/bart-large-cnn", content);
var response = await httpClient.PostAsync("https://router.huggingface.co/hf-inference/models/sshleifer/distilbart-cnn-12-6", content);
var responseContent = await response.Content.ReadAsStringAsync();

Console.WriteLine("Özet Sonucu:");
Console.WriteLine(responseContent);