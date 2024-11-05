namespace warehouse_BE.Domain.Enums;

public enum OutboundStatus
{
    Pending,     // Chưa xuất
    Processing,  // Đang xử lý
    Completed,   // Đã xuất
    Cancelled,   // Đã hủy
    OnHold       // Tạm dừng
}