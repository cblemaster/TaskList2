using Microsoft.Extensions.DependencyInjection;
using TaskList2.Data.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITaskDAO>(b => new TaskSqlDAO(GetConnectionStringFromConfiguration()));
builder.Services.AddScoped<IFolderDAO>(b => new FolderSqlDAO(GetConnectionStringFromConfiguration()));

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


static string GetConnectionStringFromConfiguration()
{
    string currentDirectory = Environment.CurrentDirectory;
    string configFileName = "appsettings.json";
    string fullPathToConfigFile = Path.Combine(currentDirectory, @"..\TaskList2.Data", configFileName);

    IConfigurationRoot builder = new ConfigurationBuilder()
        .AddJsonFile(fullPathToConfigFile, optional: false)
        .Build();

    return builder.GetConnectionString("Project") ?? string.Empty;
}