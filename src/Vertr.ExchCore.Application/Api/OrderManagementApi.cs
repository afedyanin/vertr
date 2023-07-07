using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertr.ExchCore.Domain.Abstractions;
using Vertr.ExchCore.Domain.Enums;
using Vertr.ExchCore.Domain.ValueObjects;

namespace Vertr.ExchCore.Application.Api;

public class OrderManagementApi : IOrderManagementApi
{
    private readonly IOrderCommandPublisher _commandPublisher;

    public OrderManagementApi(IOrderCommandPublisher commandPublisher)
    {
        _commandPublisher = commandPublisher;
    }

    public void CancelOrder(long orderId, int symbol, long uid)
    {
        // TODO: Implement this
        //var orderCommand = new OrderCommand(OrderCommandType.CANCEL_ORDER);
        //_commandPublisher.Publish(orderCommand);
    }

    public void CancelOrder(int serviceFlags, long eventsGroup, long timestampNs, long orderId, int symbol, long uid)
    {
        throw new NotImplementedException();
    }

    public void MoveOrder(long price, long orderId, int symbol, long uid)
    {
        throw new NotImplementedException();
    }

    public void MoveOrder(int serviceFlags, long eventsGroup, long timestampNs, long price, long orderId, int symbol, long uid)
    {
        throw new NotImplementedException();
    }

    public void OrderBookRequest(int symbolId, int depth)
    {
        throw new NotImplementedException();
    }

    public long PlaceNewOrder(int userCookie, long price, long reservedBidPrice, long size, OrderAction action, OrderType orderType, int symbol, long uid)
    {
        throw new NotImplementedException();
    }

    public void PlaceNewOrder(int serviceFlags, long eventsGroup, long timestampNs, long orderId, int userCookie, long price, long reservedBidPrice, long size, OrderAction action, OrderType orderType, int symbol, long uid)
    {
        throw new NotImplementedException();
    }

    public void ReduceOrder(long reduceSize, long orderId, int symbol, long uid)
    {
        throw new NotImplementedException();
    }

    public void ReduceOrder(int serviceFlags, long eventsGroup, long timestampNs, long reduceSize, long orderId, int symbol, long uid)
    {
        throw new NotImplementedException();
    }
}
