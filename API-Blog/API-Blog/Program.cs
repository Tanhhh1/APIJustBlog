using API.Register;
using API_Blog.Register;
var builder = WebApplication.CreateBuilder(args);

builder.Logging.RegisterLoggerServices(builder.Configuration);
builder.Services.RegisterGeneralServices(builder.Configuration);
builder.Services.AddApplicationConfiguration(builder.Configuration);
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.RegisterSwaggerServices();

var app = builder.Build();
app.RegisterGeneralApp(app.Environment);
app.RegisterSwaggerApp(builder);

await app.RegisterSeedDatabase();

app.Run();
