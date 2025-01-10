namespace Webhook.Api.Models;

public sealed record WebhookSubscription(Guid id, string EventType, string WebhookUrl, DateTime CreatedOnUtc);

public sealed record CreateWebhootRequest(string EventType, string WebhookUrl);