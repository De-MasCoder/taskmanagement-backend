using MassTransit;
using TaskEmailService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddLogging();

builder.Services.AddSingleton<Supabase.Client>(sp =>
{
    var options = new Supabase.SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true
    };
    var configuration = sp.GetRequiredService<IConfiguration>();
    var supabaseUrl = configuration["Supabase:Url"];
    var supabaseKey = configuration["Supabase:Key"];
    return new Supabase.Client(supabaseUrl, supabaseKey, options);
});

// RabbitMQ and Mass Transit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TaskCreatedEventConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? string.Empty);
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? string.Empty);
        });

        cfg.ReceiveEndpoint("task-created-event-queue", e =>
        {
            e.ConfigureConsumer<TaskCreatedEventConsumer>(ctx);
        });
    });
});


var host = builder.Build();
host.Run();
