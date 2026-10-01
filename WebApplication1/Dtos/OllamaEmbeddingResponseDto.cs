public class OllamaEmbeddingRequest
{
    public string Model { get; set; } = "";

    public string Input { get; set; } = "";
}

public class OllamaEmbeddingResponse
{
    public List<float> Embedding { get; set; } = [];
}