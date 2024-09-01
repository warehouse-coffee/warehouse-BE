using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Response;

public class ResponseDto
{
    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public object? Data { get; set; }

    public ResponseDto(int statusCode, string message, object? data = null)
    {
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    public ResponseDto(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
        Data = null;
    }
}
