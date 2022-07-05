using FileUploadApi.Common;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<FormOptions>(x => {
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    FileUploadHandler.InitDevImagePath();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();

    FileUploadHandler.InitProductionImagePath();
}

app.UseHttpsRedirection();

// app.UseStaticFiles(new StaticFileOptions()
// {
//     FileProvider = new PhysicalFileProvider(
//         Path.Combine(Directory.GetCurrentDirectory(), @".\\UploadData")),
//     RequestPath = new PathString("")
// });

app.UseAuthorization();

app.MapControllers();

app.Run();
