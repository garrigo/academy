var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//add httpclient to manager
builder.Services.AddHttpClient(
    name: "manager.api",
    configureClient: options =>
    {
        options.BaseAddress = new Uri(builder.Configuration["manager.api"]);
    }
);

//add httpclient to delivery
builder.Services.AddHttpClient(
    name: "delivery.api",
    configureClient: options =>
    {
        options.BaseAddress = new Uri(builder.Configuration["delivery.api"]);
    }
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