using Microsoft.AspNetCore.Mvc;
using System.Text;

[ApiController]
[Route("api/pdf")]
public class PdfController : ControllerBase
{
    private readonly OcrService _ocrService;
    private readonly EmbeddingService _embeddingService;
    private readonly QdrantService _qdrantService;
    private readonly TextChunkService _chunkService;

    public PdfController(
        OcrService ocrService,
        EmbeddingService embeddingService,
        QdrantService qdrantService,
        TextChunkService chunkService)
    {
        _ocrService = ocrService;
        _embeddingService = embeddingService;
        _qdrantService = qdrantService;
        _chunkService = chunkService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        Console.WriteLine("======================================");
        Console.WriteLine($"UPLOAD START : {file.FileName}");
        Console.WriteLine("======================================");

        //
        // OCR
        //

        Console.WriteLine("OCR START");

        var ocr =
            await _ocrService.ReadPdfAsync(file);

        if (ocr == null)
        {
            return BadRequest("OCR Failed.");
        }

        Console.WriteLine("OCR END");

        //
        // OCR全文作成
        //

        var text = BuildDocumentText(ocr);

        Console.WriteLine($"TEXT SIZE = {text.Length}");

        //
        // Chunk作成
        //

        var chunks =
            _chunkService.Split(
                file.FileName,
                text);

        Console.WriteLine($"CHUNK COUNT = {chunks.Count}");

        //
        // Chunk毎にEmbedding生成＆Qdrant保存
        //

        foreach (var chunk in chunks)
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine(
                $"Chunk={chunk.ChunkIndex}");

            Console.WriteLine(
                $"Chunk Size={chunk.Text.Length}");

            Console.WriteLine("Embedding START");

            List<float> embedding =
                await _embeddingService
                    .CreateEmbeddingAsync(
                        chunk.Text);

            Console.WriteLine(
                $"Embedding END : {embedding.Count}");

            Console.WriteLine("Qdrant SAVE START");

            await _qdrantService.SaveAsync(
                chunk.Filename,
                chunk.Text,
                embedding,
                chunk.ChunkIndex);

            Console.WriteLine("Qdrant SAVE END");
        }

        Console.WriteLine("======================================");
        Console.WriteLine("UPLOAD COMPLETE");
        Console.WriteLine("======================================");

        return Ok(new
        {
            FileName = file.FileName,
            TextLength = text.Length,
            ChunkCount = chunks.Count
        });
    }

    private string BuildDocumentText(OcrResponse ocr)
    {
        var sb = new StringBuilder();

        foreach (var page in ocr.Pages)
        {
            foreach (var line in page.Lines)
            {
                sb.AppendLine(line.Corrected);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}