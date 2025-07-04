using BuildingBlocks.Core.Messaging.IntegrationEvents.Catalog;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Orders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.Modules.Catalog.Data;

namespace MusicStore.Modules.Catalog.Events.Integration;

internal sealed class OrderCancelledConsumer(
    CatalogDbContext dbContext,
    IBus bus,
    ILogger<OrderCancelledConsumer> logger)
    : IConsumer<OrderCancelledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCancelledIntegrationEvent> context)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}, CorrelationId: {CorrelationId}.", 
            nameof(OrderCancelledIntegrationEvent), context.Message.Id, context.Message.CorrelationId);
        
        var productIds = context.Message.OrderItems.Select(o => o.ProductId).ToList();
        var products = await dbContext.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        foreach (var item in context.Message.OrderItems)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null) return;
            
            product.RestoreStock(item.Quantity);
        }
        
        await bus.Publish(new ProductsStockRestoredIntegrationEvent(context.Message.CorrelationId)); 

        await dbContext.SaveChangesAsync();
    }
}