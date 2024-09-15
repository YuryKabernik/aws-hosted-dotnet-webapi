using Amazon.S3;
using dotnet_intermediate_mentoring_program.Forecast;
using dotnet_intermediate_mentoring_program.HostInstance;
using dotnet_intermediate_mentoring_program.Infrastructure;
using dotnet_intermediate_mentoring_program.Options;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();
builder.WebHost.ConfigureKestrel((context, serverOptions) =>
    serverOptions.Configure(context.Configuration.GetSection("Kestrel"))
);

builder.Services.Configure<BucketOptions>(
    builder.Configuration.GetSection(BucketOptions.SectionKey)
);
builder.Services.AddDefaultAWSOptions(
    builder.Configuration.GetAWSOptions("AWS")
);
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddAWSService<IAmazonS3>();
builder.Services
    .AddNpgsql<ImagesDbContext>(
        builder.Configuration.GetConnectionString("Postgres")
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapForecasts();
app.MapRazorPages();
app.MapControllers();

app.MapHostInstanceDetails();
app.UseHealthChecks("/");

app.Run();