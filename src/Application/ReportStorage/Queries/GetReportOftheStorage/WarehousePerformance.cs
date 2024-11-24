namespace warehouse_BE.Application.ReportStorage.Queries.GetReportOftheStorage;

public class WarehousePerformance
{
    public int Id { get; set; }
    public string? WarehouseName { get; set; } // Tên kho
    public decimal Revenue { get; set; } // Doanh thu
}
