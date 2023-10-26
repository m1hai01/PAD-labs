using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserManagementAPI.Database;
using UserManagementAPI.Interfaces;
using UserManagementAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 2;
    options.Limits.MaxConcurrentUpgradedConnections = 2;
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway",
        builder =>
        {
            builder.WithOrigins("http://localhost:80")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Register ApiService and configure HttpClient
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddHttpClient<ApiService>(client =>
{
    // Configure HttpClient if needed (base address, default headers, etc.)
});

// Configure MS S   QL Server databases
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserServiceDBConnection"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Enable CORS middleware
//app.UseCors("AllowAllOrigins");
app.UseCors("AllowGateway");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();