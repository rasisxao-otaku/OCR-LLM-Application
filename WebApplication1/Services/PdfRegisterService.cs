public class PdfRegisterService
{
    private readonly PdfExtractService _extract;
    private readonly TextChunker _chunker;
    private readonly OllamaEmbeddingService _embedding;
    private readonly QdrantService _qdrant;


    public PdfRegisterService(
        PdfExtractService extract,
        TextChunker chunker,
        OllamaEmbeddingService embedding,
        QdrantService qdrant)
    {
        _extract = extract;
        _chunker = chunker;
        _embedding = embedding;
        _qdrant = qdrant;
    }


    public async Task<string> RegisterAsync(IFormFile file)
    {
        // 1. 保存
        var path = Path.Combine(
            "uploads",
            file.FileName);


        using (var stream = File.Create(path))
        {
            await file.CopyToAsync(stream);
        }


        // 2. PDF文字抽出
        var text = _extract.Extract(path);


        // 3. 分割
        var chunks = _chunker.Split(text);


        // 4. ベクトル化して保存
        foreach (var chunk in chunks)
        {
            var vector =
                await _embedding
                .CreateEmbeddingAsync(chunk);


            //await _qdrant.InsertAsync(
            //    vector,
            //    new
            //    {
            //        text = chunk,
            //        file = file.FileName
            //    });
        }


        return "登録完了";
    }
}
