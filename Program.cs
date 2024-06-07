using Microsoft.EntityFrameworkCore;
using MotdApiDotnet.Models;
using MotdApiDotnet.Services;
using MotdApiDotnet.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MotdDatabaseSettings>(builder.Configuration.GetSection("MotdDatabase"));
builder.Services.AddControllers();
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MessageOfTheDayService>();

var app = builder.Build();

await app.Services.GetRequiredService<MessageOfTheDayService>().PopulateDefaultsAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

