using Microsoft.EntityFrameworkCore;
using TestBlobStorage.Data;
using TestBlobStorage.Services;
using TestBlobStorage.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IStorageManager, BlobStorageManager>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.Configure<BlobStorageOptions>(builder.Configuration.GetSection("BlobStorage"));
// Azure SQL
//builder.Services.AddDbContext<ServerDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSql"));
//});
// Cosmos
builder.Services.AddDbContext<ServerDbContext>(options =>
{
    options.UseCosmos(
                builder.Configuration["Cosmos:Uri"],
                builder.Configuration["Cosmos:PrimaryKey"],
                builder.Configuration["Cosmos:Database"]
                );
});
builder.Services.AddDbContext<ServerDbContext>();


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
