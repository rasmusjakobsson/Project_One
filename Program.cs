var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddLogging();

builder.Services.AddHttpClient("AltaVistaSearch", (_, client) =>
{
    var token = Environment.GetEnvironmentVariable("ALTA_VISA_API_KEY") ?? string.Empty;
    client.DefaultRequestHeaders.Add("X-Api-Token", token);
});
builder.Services.AddHttpClient("ClassicSongSearch", (_, client) =>
{
    var token = Environment.GetEnvironmentVariable("CLASSIC_SONG_API_KEY") ?? string.Empty;
    client.DefaultRequestHeaders.Add("X-Api-Token", token);
});

builder.Services.AddSingleton<ISearchEngine, AltaVistaSearch>();
builder.Services.AddSingleton<ISearchEngine, ClassicSongSearch>();
builder.Services.AddSingleton<Project_One.Services.SearchHandler>();
builder.Services.AddSingleton<Project_One.Endpoints.Search>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}
// DefaultFiles must run first so "/" is rewritten to "/index.html", then StaticFiles serves it
app.UseDefaultFiles();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Incoming {Method} {Path}", context.Request.Method, context.Request.Path);
    await next(context);
});

// Resolve and register endpoints
var search = app.Services.GetRequiredService<Project_One.Endpoints.Search>();
search.RegisterEndpoints(app);

app.Run();