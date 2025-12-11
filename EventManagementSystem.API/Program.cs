var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api", () =>
{
    return Results.Ok(new
    {
        message = "EMS API",
        status = "OK"
    });
});

app.MapGet("/protected", () =>
{
    return Results.Ok(new
    {
        message = "....................",
        status = "AUTHORIZED"
    });
});

app.Run();