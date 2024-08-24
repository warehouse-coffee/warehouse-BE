namespace warehouse_BE.Domain.Entities;

public class Product : BaseAuditableEntity
{
        public int ProductId { get; set; }
        public string? Name { get; set; }

}

