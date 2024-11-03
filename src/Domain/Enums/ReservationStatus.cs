using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Domain.Enums;

public enum ReservationStatus
{
    Pending,        // Đơn hàng đang chờ xác nhận
    Confirmed,      // Đơn hàng đã xác nhận
    Cancelled,      // Đơn hàng bị hủy
    PickedUp        // Đơn hàng đã được lấy
}
