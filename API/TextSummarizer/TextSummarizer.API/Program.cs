using TextSummarizer.API.Services;
using TextSummarizer.API.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Bind Bytez settings
builder.Services.Configure<BytezSettings>(builder.Configuration.GetSection("Bytez"));


// Register HttpClient and service
builder.Services.AddHttpClient<ITextSummarizer, BytezTextSummarizer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
