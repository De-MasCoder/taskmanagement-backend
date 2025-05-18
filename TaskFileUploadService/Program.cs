using MassTransit;
using TaskFileUploadService;
using TaskFileUploadService.Consumers;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

// RabbitMQ and Mass Transit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<FileUploadConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? string.Empty);
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? string.Empty);
        });

        cfg.ReceiveEndpoint("task-attachment-upload-event-queue", e =>
        {
            e.ConfigureConsumer<FileUploadConsumer>(ctx);
        });
    });
});

// Environment-aware DI registration
builder.Services.AddTransient<IFileStorageService>(provider =>
{
    var env = provider.GetRequiredService<IHostEnvironment>();
    var config = provider.GetRequiredService<IConfiguration>();

    return env.IsProduction()
        ? new AzureBlobStorageService(config)
        : new SupabaseStorageService(config);
});



var host = builder.Build();
host.Run();
