using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{

    private readonly RagService _rag;


    public ChatController(
        RagService rag)
    {
        _rag = rag;
    }



    [HttpPost]
    public async Task<IActionResult> Chat(
        ChatRequest request)
    {

        var filter = new DocumentSearchFilter
        {
            DocumentType = request.DocumentType,
            VehicleModel = request.VehicleModel,
            Manufacturer = request.Manufacturer,
            Year = request.Year,
            PartNumber = request.PartNumber,
            Limit = request.Limit
        };

        var result =
            await _rag
            .AskAsync(request.Question, filter);


        return Ok(new
        {
            result,
            filter
        });
    }
}