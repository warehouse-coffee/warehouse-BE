﻿using warehouse_BE.Application.Customer.Commands.UpdateCustomer;
using warehouse_BE.Application.Orders.Commands.DeleteOrder;
using warehouse_BE.Application.Orders.Commands.ImportStorage;
using warehouse_BE.Application.Orders.Queries.GetOrderDetail;
using warehouse_BE.Application.Orders.Queries.GetOrderList;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints;

public class Orders : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(ImportProduct,"/import")
            .MapPost(GetList,"/all")
            .MapDelete(DeleteOrder,"/{id}")
            .MapGet(GetOrderDetail, "/{id}")
            ;
    }
    public Task<ResponseDto> ImportProduct(ISender sender, ImportStogareCommand command)
    {
        return sender.Send(command);
    }
    public Task<OrderListVM> GetList(ISender sender, GetOrderListQuery query)
    {
        return sender.Send(query);
    }
    public Task<bool> DeleteOrder(ISender sender, string id)
    {
        return sender.Send(new DeleteOrderCommand { OrderId = id });
    }
    public Task<OrderDetailVM> GetOrderDetail(ISender sender, string id)
    {
        return sender.Send(new GetOrderDetailQuery { OrderId = id });
    }
}