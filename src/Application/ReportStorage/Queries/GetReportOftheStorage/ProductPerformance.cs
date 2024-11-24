using warehouse_BE.Application.Storages.Queries.GetStorageOfUser;

namespace warehouse_BE.Application.ReportStorage.Queries.GetReportOftheStorage;

public class ProductPerformance
{
    public int StorageId {  get; set; } 
    public string? ProductName { get; set; } // Tên sản phẩm
    public int TotalSold { get; set; } // Số lượng bán ra
    public double AverageStorageTime { get; set; } // Thời gian lưu kho trung bình
}
