using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using prj_import_biznes.Services.Import;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("ViteDev", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 100 * 1024 * 1024;
});

builder.Services.AddScoped<prj_import_biznes.Services.Cartera.ICarteraService,
                           prj_import_biznes.Services.Cartera.CarteraService>();

builder.Services.AddScoped<IChannelImportService, SmsImportService>();
builder.Services.AddScoped<IChannelImportService, IvrImportService>(); // <—
builder.Services.AddScoped<IChannelImportService, EmailImportService>();
builder.Services.AddScoped<IChannelImportService, WapiImportService>();
builder.Services.AddScoped<IChannelImportService, BotImportService>();
builder.Services.AddScoped<IImportServiceFactory, ImportServiceFactory>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Fuerza a Swagger a mostrar ChannelType como string con estos 3 valores
    c.MapType<ChannelType>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = new List<IOpenApiAny>
        {
            new Microsoft.OpenApi.Any.OpenApiString("SMS"),
            new Microsoft.OpenApi.Any.OpenApiString("IVR"),
            new Microsoft.OpenApi.Any.OpenApiString("EMAIL"),
            new Microsoft.OpenApi.Any.OpenApiString("WAPI"),
            new Microsoft.OpenApi.Any.OpenApiString("BOT")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("ViteDev");

app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
