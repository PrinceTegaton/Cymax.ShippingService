using CymaxStore.API.Services;
using ParagoLogistics.API.DTO;
using Rext;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen(a =>
{
    // set the comments path for the Swagger JSON and UI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    a.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IRextHttpClient>(a => new RextHttpClient(new RextHttpCongifuration
{
    RelaxSslCertValidation = true,
    DeserializeSuccessResponseOnly = false
}));

// register shipping config >> providers
var shippingConfig = builder.Configuration.GetSection(nameof(ShippingConfig)).Get<ShippingConfig>();
builder.Services.AddSingleton(shippingConfig);

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