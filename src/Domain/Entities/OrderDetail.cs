﻿namespace warehouse_BE.Domain.Entities;

public class OrderDetail : BaseAuditableEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPice { get; set; }

    // Relationships
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}