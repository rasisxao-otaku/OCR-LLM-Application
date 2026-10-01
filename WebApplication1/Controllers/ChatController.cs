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

        var result =
            await _rag
            .AskAsync(request.Question);


        return Ok(new
        {
            result
        });
    }
}