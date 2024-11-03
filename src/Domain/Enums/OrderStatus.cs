using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Domain.Enums;

public enum OrderStatus
{
    Pending,            // Đang chờ xử lý
    AwaitingShipment,   // Chờ xuất kho
    Shipped,            // Đã xuất kho
    InTransit,          // Đang vận chuyển
    Completed,          // Hoàn thành
    Cancelled           // Đã hủy (nếu có thêm trường hợp hủy đơn hàng)
}
