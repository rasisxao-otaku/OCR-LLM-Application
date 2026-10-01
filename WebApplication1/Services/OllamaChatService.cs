using System.Net.Http.Json;

public class OllamaChatService
{
    private readonly HttpClient _http;


    public OllamaChatService(
        HttpClient http)
    {
        _http = http;
    }



    public async Task<string> ChatAsync(
        string prompt)
    {

        var request = new
        {
            model = "llama3",
            prompt = prompt,
            stream = false
        };


        var response =
            await _http.PostAsJsonAsync(
                "api/generate",
                request);


        response.EnsureSuccessStatusCode();


        var result =
            await response.Content
            .ReadFromJsonAsync<OllamaChatResponse>();


        return result!.Response;
    }
}



public class OllamaChatResponse
{
    public string Response { get; set; } = "";
}