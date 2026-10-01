public class QdrantSearchResponse
{
    public List<QdrantResult> Result { get; set; }
}


public class QdrantResult
{
    public float Score { get; set; }

    public Dictionary<string, string> Payload { get; set; }
}