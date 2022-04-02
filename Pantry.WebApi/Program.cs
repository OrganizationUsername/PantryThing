using Pantry.Data;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

new DataBase().Database.EnsureCreated();
builder.Services.AddDbContext<DataBase>();
var logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.File(@"..\..\..\..\PantryLogs.log")
    .CreateLogger();

builder.Services.AddSingleton<Logger>(logger);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
