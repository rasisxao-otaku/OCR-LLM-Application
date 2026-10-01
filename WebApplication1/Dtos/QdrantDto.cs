public class QdrantPoint
{
    public long Id { get; set; }

    public List<float> Vector { get; set; } = [];

    public object Payload { get; set; } = default!;
}

public class QdrantUpsertRequest
{
    public List<QdrantPoint> Points
    { get; set; } = [];
}