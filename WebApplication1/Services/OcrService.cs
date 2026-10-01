using System.Net.Http.Headers;
using System.Text.Json;

public class OcrService
{
    private readonly HttpClient _http;

    public OcrService(HttpClient http)
    {
        _http = http;
    }


    public async Task<OcrResponse?> ReadPdfAsync(
        IFormFile file)
    {

        using var content = new MultipartFormDataContent();


        using var stream =
            file.OpenReadStream();


        var fileContent =
            new StreamContent(stream);


        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(
                "application/pdf"
            );


        content.Add(
            fileContent,
            "file",
            file.FileName
        );


        var response =
            await _http.PostAsync(
                "ocr",
                content
            );


        response.EnsureSuccessStatusCode();


        var json =
            await response.Content.ReadAsStringAsync();


        return JsonSerializer.Deserialize<OcrResponse>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}