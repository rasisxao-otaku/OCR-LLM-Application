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
     int chunkIndex,
     DocumentMetadata? metadata = null)
    {
        await InitializeAsync();

        var payload = new
        {
            filename,
            text,
            chunkIndex,
            documentType = metadata?.DocumentType ?? "manual",
            vehicleModel = metadata?.VehicleModel,
            manufacturer = metadata?.Manufacturer,
            year = metadata?.Year,
            partNumber = metadata?.PartNumber,
            uploadedAtUtc = metadata?.UploadedAtUtc ?? DateTime.UtcNow
        };

        var request = new
        {
            points = new[]
            {
            new
            {
                id = Guid.NewGuid(),

                vector = embedding,
                payload
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
       List<float> vector,
       DocumentSearchFilter? filter = null)
    {
        var must = new List<object>();

        if (!string.IsNullOrWhiteSpace(filter?.DocumentType))
        {
            must.Add(new
            {
                key = "documentType",
                match = new { value = filter.DocumentType }
            });
        }

        if (!string.IsNullOrWhiteSpace(filter?.VehicleModel))
        {
            must.Add(new
            {
                key = "vehicleModel",
                match = new { value = filter.VehicleModel }
            });
        }

        if (!string.IsNullOrWhiteSpace(filter?.Manufacturer))
        {
            must.Add(new
            {
                key = "manufacturer",
                match = new { value = filter.Manufacturer }
            });
        }

        if (filter?.Year is not null)
        {
            must.Add(new
            {
                key = "year",
                match = new { value = filter.Year.Value }
            });
        }

        if (!string.IsNullOrWhiteSpace(filter?.PartNumber))
        {
            must.Add(new
            {
                key = "partNumber",
                match = new { value = filter.PartNumber }
            });
        }

        var request = new Dictionary<string, object>
        {
            ["vector"] = vector,
            ["limit"] = filter?.Limit ?? 5,
            ["with_payload"] = true
        };

        if (must.Count > 0)
        {
            request["filter"] = new { must };
        }

        var response =
            await _httpClient.PostAsJsonAsync(
                $"/collections/{CollectionName}/points/search",
                request
            );

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}