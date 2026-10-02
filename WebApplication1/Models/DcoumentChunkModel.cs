public class DocumentChunk
{
    public string Filename { get; set; } = "";
    public int ChunkIndex { get; set; }
    public string Text { get; set; } = "";
    public string DocumentType { get; set; } = "manual";
    public string? VehicleModel { get; set; }
    public string? Manufacturer { get; set; }
    public int? Year { get; set; }
    public string? PartNumber { get; set; }
}

public class DocumentMetadata
{
    public string Filename { get; set; } = "";
    public string DocumentType { get; set; } = "manual";
    public string? VehicleModel { get; set; }
    public string? Manufacturer { get; set; }
    public int? Year { get; set; }
    public string? PartNumber { get; set; }
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
}

public class DocumentUploadRequest
{
    public string? DocumentType { get; set; } = "manual";
    public string? VehicleModel { get; set; }
    public string? Manufacturer { get; set; }
    public int? Year { get; set; }
    public string? PartNumber { get; set; }
}

public class DocumentSearchFilter
{
    public string? DocumentType { get; set; }
    public string? VehicleModel { get; set; }
    public string? Manufacturer { get; set; }
    public int? Year { get; set; }
    public string? PartNumber { get; set; }
    public int Limit { get; set; } = 5;
}