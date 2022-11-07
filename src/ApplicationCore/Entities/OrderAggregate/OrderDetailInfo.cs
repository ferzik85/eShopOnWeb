using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

public class OrderDetailInfo
{
    public virtual string id { get; set; }
    public string BuyerId { get; set; }
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
    public Address ShipToAddress { get; set; }

    public List<OrderItem> OrderItems = new List<OrderItem>();

    public static OrderDetailInfo FromOrder(Order order) {

        var cosmosDetailInfo = new OrderDetailInfo()
        {
            OrderItems = order.OrderItems.ToList(),
            ShipToAddress = order.ShipToAddress,
            BuyerId = order.BuyerId,
            OrderDate = order.OrderDate,
            id = order.Id.ToString()
        };

        return cosmosDetailInfo;
    }
}
