using lab07.Models;

var builder = WebApplication.CreateBuilder(args);


// Aggiungi da appsettings.json (o user-secrets) una configurazione basata su BookStoreDatabaseSettings.cs
// Crea una istanza BookStoreDatabaseSettings che contiene già tutti i settaggi letti
// Permette a diversi posti di ottenere questa informazione senza leggere appsettings.json ogni volta
// La configurazione viene eseguita dal builder.Build();
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("LogEntryDatabase")
);
// Add services to the container.
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

app.UseAuthorization();

app.MapControllers();

app.Run();
