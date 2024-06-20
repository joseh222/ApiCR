using BackParroquia.Repositories;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Add-MyConnection
string? strConnection = builder.Configuration.GetConnectionString("MyConnection");
builder.Services.AddSingleton<IDbConnection>((sp) => new SqlConnection(strConnection));
builder.Services.AddScoped<ITipoMisaRepository, TipoMisaRepository>();
builder.Services.AddScoped<IMotivoMisaRepository, MotivoMisaRepository>();
builder.Services.AddScoped<IMisaRepository, MisaRepository>();
builder.Services.AddScoped<INombresRepository, NombresRepository>();

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
}

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

app.Run();
