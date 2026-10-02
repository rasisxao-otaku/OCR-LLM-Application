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
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromForm] DocumentUploadRequest? request)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("The file field is required.");
        }

        var metadata = new DocumentMetadata
        {
            Filename = file.FileName,
            DocumentType = request?.DocumentType ?? "manual",
            VehicleModel = request?.VehicleModel,
            Manufacturer = request?.Manufacturer,
            Year = request?.Year,
            PartNumber = request?.PartNumber
        };

        Console.WriteLine("======================================");
        Console.WriteLine($"UPLOAD START : {file.FileName}");
        Console.WriteLine($"DOCUMENT TYPE : {metadata.DocumentType}");
        Console.WriteLine("======================================");

        Console.WriteLine("OCR START");

        var ocr =
            await _ocrService.ReadPdfAsync(file);

        if (ocr == null)
        {
            return BadRequest("OCR Failed.");
        }

        Console.WriteLine("OCR END");

        var text = BuildDocumentText(ocr);

        Console.WriteLine($"TEXT SIZE = {text.Length}");

        var chunks =
            _chunkService.Split(
                file.FileName,
                text);

        Console.WriteLine($"CHUNK COUNT = {chunks.Count}");

        foreach (var chunk in chunks)
        {
            chunk.DocumentType = metadata.DocumentType;
            chunk.VehicleModel = metadata.VehicleModel;
            chunk.Manufacturer = metadata.Manufacturer;
            chunk.Year = metadata.Year;
            chunk.PartNumber = metadata.PartNumber;

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
                chunk.ChunkIndex,
                new DocumentMetadata
                {
                    Filename = chunk.Filename,
                    DocumentType = chunk.DocumentType,
                    VehicleModel = chunk.VehicleModel,
                    Manufacturer = chunk.Manufacturer,
                    Year = chunk.Year,
                    PartNumber = chunk.PartNumber
                });

            Console.WriteLine("Qdrant SAVE END");
        }

        Console.WriteLine("======================================");
        Console.WriteLine("UPLOAD COMPLETE");
        Console.WriteLine("======================================");

        return Ok(new
        {
            FileName = file.FileName,
            TextLength = text.Length,
            ChunkCount = chunks.Count,
            DocumentType = metadata.DocumentType,
            VehicleModel = metadata.VehicleModel,
            Manufacturer = metadata.Manufacturer,
            Year = metadata.Year,
            PartNumber = metadata.PartNumber
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