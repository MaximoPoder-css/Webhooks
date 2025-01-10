using Webhook.Api.Models;

namespace Webhook.Api.Repositories;

internal sealed class InMemoryWebhookSubscriptionRepository
{
    private readonly List<WebhookSubscription> _orders =[];

    public void Add(WebhookSubscription order){
        _orders.Add(order);
    }

    public IReadOnlyList<WebhookSubscription> GetByEventType(string EventType){
        return _orders.Where(s=> s.EventType == EventType).ToList().AsReadOnly();
    }
}