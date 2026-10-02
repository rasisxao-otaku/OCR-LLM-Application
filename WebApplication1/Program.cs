using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);


// Controller
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// OpenAPI
builder.Services.AddOpenApi();


// ==========================
// Ollama
// ==========================

// Embedding用
builder.Services.AddHttpClient<EmbeddingService>(c =>
{
    c.BaseAddress =
        new Uri("http://ollama:11434/");
});


// Chat生成用
builder.Services.AddHttpClient<OllamaChatService>(c =>
{
    c.BaseAddress =
        new Uri("http://ollama:11434/");
});



// ==========================
// Qdrant
// ==========================

builder.Services.AddHttpClient<QdrantService>(c =>
{
    c.BaseAddress =
        new Uri("http://qdrant:6333/");
});



// ==========================
// OCR
// ==========================

builder.Services.AddHttpClient<OcrService>(c =>
{
    c.BaseAddress =
        new Uri("http://ocr:8000/");
});



// ==========================
// Application Services
// ==========================

builder.Services.AddScoped<RagService>();

builder.Services.AddScoped<PdfExtractService>();

builder.Services.AddScoped<TextChunker>();

builder.Services.AddScoped<PdfRegisterService>();

builder.Services.AddSingleton<TextChunkService>();

builder.Services.AddScoped<SearchService>();

var app = builder.Build();

app.UseCors("FrontendPolicy");

// Docker環境でもSwagger公開
app.MapOpenApi();

app.MapScalarApiReference();



app.MapControllers();


app.Run();