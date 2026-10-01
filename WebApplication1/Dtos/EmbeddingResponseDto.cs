public record ChatRequest(string Question);

public record EmbeddingResponse(
    float[] Embedding
);