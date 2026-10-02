public record ChatRequest
{
    public string Question { get; set; } = "";
    public string? DocumentType { get; set; }
    public string? VehicleModel { get; set; }
    public string? Manufacturer { get; set; }
    public int? Year { get; set; }
    public string? PartNumber { get; set; }
    public int Limit { get; set; } = 5;
}

public record EmbeddingResponse(
    float[] Embedding
);