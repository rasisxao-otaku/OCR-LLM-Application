using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly SearchService _searchService;

    public TestController(
        SearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] TestRequest request)
    {
        var result =
            await _searchService.SearchAsync(
                request.Text);

        return Ok(result);
    }
}

public class TestRequest
{
    public string Text { get; set; } = "";
}