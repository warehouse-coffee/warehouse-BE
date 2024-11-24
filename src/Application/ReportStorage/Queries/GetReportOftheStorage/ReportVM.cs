namespace warehouse_BE.Application.ReportStorage.Queries.GetReportOftheStorage;

public class ReportVM
{
    public ReportVM()
    {
        WarehouseStatistics = new List<WarehousePerformance>();
        ImportStatistics = new List<ImportSummary>();
        TopProducts = new List<ProductPerformance>();
        SlowMovingProducts = new List<ProductPerformance>();
    }
    public required List<WarehousePerformance> WarehouseStatistics { get; set; } // Hiệu suất hoạt động các kho

    // Thống kê đơn hàng
    public int TotalOrders { get; set; } // Tổng số đơn hàng

    // Thống kê nhập hàng
    public required List<ImportSummary> ImportStatistics { get; set; } // Tổng quan nhập hàng

    // Thống kê sản phẩm
    public required List<ProductPerformance> TopProducts { get; set; } // Danh sách sản phẩm bán chạy
    public required List<ProductPerformance> SlowMovingProducts { get; set; } // Danh sách sản phẩm tồn lâu

    // Lợi nhuận
    public decimal TotalRevenue { get; set; } // Tổng doanh thu
    public decimal TotalImportCost { get; set; } 

}
