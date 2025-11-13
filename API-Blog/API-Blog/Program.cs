using API_Blog.Register;
var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterGeneralServices();
builder.Services.AddApplicationConfiguration();
builder.Services.AddDatabaseConfiguration(builder.Configuration);

var app = builder.Build();
app.RegisterGeneralApp(app.Environment);

app.Run();
