using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
public class CosmosOrder
{
    public virtual string id { get; set; }
    public string BuyerId { get; set; }
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
    public Address ShipToAddress { get; set; }

    public List<OrderItem> OrderItems = new List<OrderItem>();

    public static CosmosOrder FromOrder(Order order) {

        var cosmosOrder = new CosmosOrder()
        {
            OrderItems = order.OrderItems.ToList(),
            ShipToAddress = order.ShipToAddress,
            BuyerId = order.BuyerId,
            OrderDate = order.OrderDate,
            id = order.Id.ToString()
        };

        return cosmosOrder;
    }
    public decimal Total()
    {
        var total = 0m;
        foreach (var item in OrderItems)
        {
            total += item.UnitPrice * item.Units;
        }
        return total;
    }
}
