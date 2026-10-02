public class RagService
{

    private readonly EmbeddingService _embedding;
    private readonly QdrantService _qdrant;
    private readonly OllamaChatService _chat;


    public RagService(
        EmbeddingService embedding,
        QdrantService qdrant,
        OllamaChatService chat)
    {
        _embedding = embedding;
        _qdrant = qdrant;
        _chat = chat;
    }



    public async Task<string> AskAsync(
    string question,
    DocumentSearchFilter? filter = null)
    {

        // 質問をベクトル化
        var vector =
            await _embedding
            .CreateEmbeddingAsync(question);



        // Qdrant検索
        var context =
            await _qdrant
            .SearchAsync(vector, filter);



        var prompt = $"""

以下の資料を参考に回答してください。

資料:
{context}


質問:
{question}

回答:
""";



        return await _chat
            .ChatAsync(prompt);
    }

}