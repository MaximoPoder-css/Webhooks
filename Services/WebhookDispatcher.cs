using Webhook.Api.Models;
using Webhook.Api.Repositories;

namespace Webhook.Api.Services;

internal sealed class WebhookDispatcher(HttpClient httpClient, InMemoryWebhookSubscriptionRepository subscriptionRepository) {
    
    public async Task DispatchAsync(string EventType, object payload){ 
        
        var subscriptions = subscriptionRepository.GetByEventType(EventType);

        foreach(WebhookSubscription subscription in subscriptions){
            
            var request = new {
                Id = Guid.NewGuid(),
                subscription.EventType,
                SubscriptionId = subscription.id,
                Timestamp = DateTime.UtcNow,
                Data = payload
            };

            await httpClient.PostAsJsonAsync(subscription.WebhookUrl, request);
        }
    }
}