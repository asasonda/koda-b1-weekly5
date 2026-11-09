using Ewallet.Api.Repositories;
using Ewallet.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<IUserServices, UserServices>();

builder.Services.AddScoped<IWalletRepository, SqlWalletRepository>();
builder.Services.AddScoped<IWalletServices, WalletServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
