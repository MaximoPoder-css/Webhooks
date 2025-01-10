using Webhook.Api.Models;
using Webhook.Api.Repositories;
using Webhook.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<InMemoryOrderRepository>();
builder.Services.AddSingleton<InMemoryWebhookSubscriptionRepository>();

builder.Services.AddHttpClient<WebhookDispatcher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Webhook api");
    });
}


app.UseHttpsRedirection();

var webhookGroup = app.MapGroup("webhook/").WithTags("webhook");

webhookGroup.MapPost("webhook/subscription", (WebhookSubscription request, InMemoryWebhookSubscriptionRepository repository) =>
{
    var webhookSubscription = new WebhookSubscription(Guid.NewGuid(), request.EventType, request.WebhookUrl, DateTime.UtcNow);

    repository.Add(webhookSubscription);

    return Results.Ok(webhookSubscription);
});


var orderGroup = app.MapGroup("/order").WithTags("order");

orderGroup.MapPost("/", async(CreateOrderRequest request, InMemoryOrderRepository repository, WebhookDispatcher webhookDispatcher) =>
{
    var order = new Order(Guid.NewGuid(), request.CustomerName, request.Amount, DateTime.UtcNow);

    repository.Add(order);

    await webhookDispatcher.DispatchAsync("order.created", order);

    return Results.Ok(order);
});


orderGroup.MapGet("/", (InMemoryOrderRepository respository) =>
{
    return Results.Ok(respository.GetAll());
});

app.Run();