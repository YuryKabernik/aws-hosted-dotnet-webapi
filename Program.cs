using dotnet_intermediate_mentoring_program.Forecast;
using dotnet_intermediate_mentoring_program.HostInstance;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();
builder.WebHost.ConfigureKestrel((context, serverOptions) =>
    serverOptions.Configure(context.Configuration.GetSection("Kestrel"))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapForecasts();
app.MapHostInstanceDetails();
app.UseHealthChecks("/");

app.Run();
