using Microsoft.EntityFrameworkCore;
using UserService;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserServiceContext>
    (options => options.UseSqlServer(@"Data Source=localhost;Initial Catalog=UserService;User ID=sa;Password=P@ssw0rd;"));

builder.Services.AddSingleton<IntegrationEventSenderService>();

builder.Services.AddHostedService
    (provider => provider.GetService<IntegrationEventSenderService>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
