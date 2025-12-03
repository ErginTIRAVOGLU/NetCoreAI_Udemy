using System.Net.Http.Headers;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;
using NAudio.Wave;

const string apiKey = "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";

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("🤖 OpenAI Speech Chat Bot Uygulamasına Hoş Geldin. Konuşmak için enter tuşuna basabilirsin.\n");

while (true)
{
    Console.ReadLine();
    string audioFilePath = "recorded_audio.wav";

    Console.WriteLine("🎤 Ses kaydı başlıyor... Lütfen konuşun.");
    RecordAudio(audioFilePath);
    Console.WriteLine("⏹️ Ses kaydı tamamlandı.");

    string transcription = await TranscribeAudiAsync(audioFilePath);
    Console.WriteLine($"\n💬 Transkripsiyon: {transcription}\n");

    string reply = await AskChatGptAsync(transcription);
    Console.WriteLine($"\n🤖 ChatGPT Yanıtı: {reply}\n");

    var synth = new SpeechSynthesizer();
    synth.Speak(reply);
}

static void RecordAudio(string outputFilePath)
{
    using var waveIn = new WaveInEvent();
    waveIn.WaveFormat = new WaveFormat(16000, 1);
    using var writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
    waveIn.DataAvailable += (s, a) => writer.Write(a.Buffer, 0, a.BytesRecorded);
    waveIn.StartRecording();
    Thread.Sleep(10000);
    waveIn.StopRecording();
}

static async Task<string> TranscribeAudiAsync(string audioFilePath)
{
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

    using var form = new MultipartFormDataContent();
    using var fileStream = File.OpenRead(audioFilePath);
    form.Add(new StreamContent(fileStream), "file", Path.GetFileName(audioFilePath));
    form.Add(new StringContent("whisper-1"), "model");

    var response = await httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);
    response.EnsureSuccessStatusCode();

    var jsonResponse = await response.Content.ReadAsStringAsync();
    var jsonDoc = JsonDocument.Parse(jsonResponse);
    return jsonDoc.RootElement.GetProperty("text").GetString() ?? string.Empty;
}

static async Task<string> AskChatGptAsync(string prompt)
{
    using var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

    var requestBody = new
    {
        model = "gpt-4",
        messages = new[]
        {
                new
                {
                    role = "system",
                    content = "Sen nazik ve yardımcı bir asistansın."
                },
                new
                {
                    role = "user",
                    content = prompt
                }
            },
        temperature = 0.7
    };

    var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
    var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);
    response.EnsureSuccessStatusCode();

    var responseString = await response.Content.ReadAsStringAsync();
    var responseJson = JsonDocument.Parse(responseString);

    return responseJson
        .RootElement
        .GetProperty("choices")[0]
        .GetProperty("message")
        .GetProperty("content")
        .GetString() ?? string.Empty;
}