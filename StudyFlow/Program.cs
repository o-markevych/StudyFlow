using StudyFlow.Components;
using StudyFlow.Services;
using StudyFlow.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure settings
builder.Services.Configure<StudyFlowSettings>(
    builder.Configuration.GetSection("StudyFlow"));

// Add HttpClient factory
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("OpenAI", client =>
{
    // TODO: Configure with Azure OpenAI or OpenAI API endpoint
    client.BaseAddress = new Uri("https://api.openai.com/v1/");
    // client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});

// Register StudyFlow services
builder.Services.AddSingleton<IDocumentService, DocumentService>();
builder.Services.AddSingleton<IPdfProcessingService, PdfProcessingService>();
builder.Services.AddSingleton<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<IStudyContentGenerator, StudyContentGenerator>();
builder.Services.AddScoped<IWebEnrichmentService, WebEnrichmentService>();
builder.Services.AddSingleton<ISpacedRepetitionService, SpacedRepetitionService>();
builder.Services.AddScoped<StudyFlowOrchestrator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
