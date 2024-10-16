

namespace warehouse_BE.Domain.Exceptions;

public class UnsupportedConfigurationKeyException : Exception
{
    public UnsupportedConfigurationKeyException(string key)
        : base($"The configuration key '{key}' is not supported.")
    {
    }
}