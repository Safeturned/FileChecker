using Safeturned.FileChecker;
using dnlib.DotNet;

using FeatureResult = Safeturned.FileChecker.FeatureResult;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", version = Checker.Version }));

app.MapGet("/version", () => Results.Ok(new { version = Checker.Version }));

app.MapPost("/analyze", async (HttpRequest request) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest(new { error = "Expected multipart/form-data" });

    var form = await request.ReadFormAsync();
    var file = form.Files.GetFile("file");

    if (file is null || file.Length == 0)
        return Results.BadRequest(new { error = "No file provided" });

    using var stream = new MemoryStream();
    await file.CopyToAsync(stream);
    stream.Position = 0;

    try
    {
        var result = Checker.Process(stream);

        stream.Position = 0;
        var metadata = ExtractMetadata(stream);

        return Results.Ok(new AnalyzeResponse(
            result.Score,
            Checker.Version,
            result.Features.ToArray(),
            metadata
        ));
    }
    catch (BadImageFormatException)
    {
        return Results.BadRequest(new { error = "Invalid .NET assembly" });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Analysis failed: {ex.Message}");
    }
});

app.MapPost("/validate", async (HttpRequest request) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest(new { error = "Expected multipart/form-data" });

    var form = await request.ReadFormAsync();
    var file = form.Files.GetFile("file");

    if (file is null || file.Length == 0)
        return Results.BadRequest(new { error = "No file provided" });

    using var stream = new MemoryStream();
    await file.CopyToAsync(stream);
    stream.Position = 0;

    try
    {
        var module = ModuleDefMD.Load(stream);
        return Results.Ok(new { valid = module != null });
    }
    catch
    {
        return Results.Ok(new { valid = false });
    }
});

app.Run();

static AssemblyMetadata ExtractMetadata(Stream fileStream)
{
    try
    {
        fileStream.Position = 0;
        var module = ModuleDefMD.Load(fileStream);
        var assembly = module.Assembly;

        if (assembly?.CustomAttributes == null)
            return new AssemblyMetadata();

        string? company = null, product = null, title = null, copyright = null;

        foreach (var attr in assembly.CustomAttributes)
        {
            if (attr.ConstructorArguments.Count == 0)
                continue;

            var value = attr.ConstructorArguments[0].Value?.ToString();
            if (string.IsNullOrWhiteSpace(value))
                continue;

            switch (attr.TypeFullName)
            {
                case "System.Reflection.AssemblyCompanyAttribute":
                    company = value;
                    break;
                case "System.Reflection.AssemblyProductAttribute":
                    product = value;
                    break;
                case "System.Reflection.AssemblyTitleAttribute":
                    title = value;
                    break;
                case "System.Reflection.AssemblyCopyrightAttribute":
                    copyright = value;
                    break;
            }
        }

        return new AssemblyMetadata
        {
            Company = company,
            Product = product,
            Title = title,
            Copyright = copyright,
            Guid = module.Mvid?.ToString()
        };
    }
    catch
    {
        return new AssemblyMetadata();
    }
}

public record AnalyzeResponse(
    float Score,
    string Version,
    FeatureResult[] Features,
    AssemblyMetadata Metadata
);

public record AssemblyMetadata
{
    public string? Company { get; init; }
    public string? Product { get; init; }
    public string? Title { get; init; }
    public string? Copyright { get; init; }
    public string? Guid { get; init; }
}
