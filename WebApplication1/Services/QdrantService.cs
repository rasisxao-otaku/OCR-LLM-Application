using System.Net.Http.Json;
using static System.Net.WebRequestMethods;


public class QdrantService
{
    private readonly HttpClient _httpClient;

    private const string CollectionName = "manuals";


    public QdrantService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }



    public async Task InitializeAsync()
    {
        var check =
            await _httpClient.GetAsync(
                $"/collections/{CollectionName}"
            );


        if (check.IsSuccessStatusCode)
        {
            Console.WriteLine(
                "Qdrant collection exists"
            );

            return;
        }


        Console.WriteLine(
            "Creating Qdrant collection..."
        );


        var body = new
        {
            vectors = new
            {
                size = 768,
                distance = "Cosine"
            }
        };


        var response =
            await _httpClient.PutAsJsonAsync(
                $"/collections/{CollectionName}",
                body
            );


        var result =
            await response.Content.ReadAsStringAsync();


        Console.WriteLine(result);


        response.EnsureSuccessStatusCode();
    }





    public async Task SaveAsync(
     string filename,
     string text,
     List<float> embedding,
     int chunkIndex)
    {
        await InitializeAsync();

        var request = new
        {
            points = new[]
            {
            new
            {
                id = Guid.NewGuid(),

                vector = embedding,
                payload = new
                {
                    filename,
                    text,
                    chunkIndex
                }
            }
        }
        };

        var response =
            await _httpClient.PutAsJsonAsync(
                $"/collections/{CollectionName}/points",
                request
            );

        response.EnsureSuccessStatusCode();
    }

    public async Task<string> SearchAsync(
       List<float> vector)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                $"/collections/{CollectionName}/points/search",
                new
                {
                    vector,
                    limit = 3,
                    with_payload = true
                });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}