using AsyncMessaging;
using UseCases;
using dotnet_intermediate_mentoring_program.Forecast;
using dotnet_intermediate_mentoring_program.HostInstance;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();
builder.WebHost.ConfigureKestrel((context, serverOptions) =>
    serverOptions.Configure(context.Configuration.GetSection("Kestrel"))
);
builder.Services.AddDefaultAWSOptions(
    builder.Configuration.GetAWSOptions("AWS")
);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.RegisterMessaging(builder.Configuration);
builder.Services.RegisterUseCases(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapForecasts();
app.MapControllers();

app.MapHostInstanceDetails();
app.UseHealthChecks("/");

app.Run();