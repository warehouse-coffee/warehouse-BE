using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Domain.ValueObjects;

public class ConfigurationKeys : ValueObject
{
    public const string WebServiceUrl = "WebServiceUrl";
    public const string AIServiceKey = "AIServiceKey";
    public const string EmailServiceKey = "EmailServiceKey";

    public static ConfigurationKeys From(string key)
    {
        var configurationKey = new ConfigurationKeys(key);

        if (!SupportedKeys.Contains(configurationKey))
        {
            throw new UnsupportedConfigurationKeyException(key);
        }

        return configurationKey;
    }

    public string Key { get; private set; }

    private ConfigurationKeys(string key)
    {
        Key = string.IsNullOrWhiteSpace(key) ? throw new ArgumentNullException(nameof(key)) : key;
    }

    public static implicit operator string(ConfigurationKeys configurationKey)
    {
        return configurationKey.ToString();
    }

    public static explicit operator ConfigurationKeys(string key)
    {
        return From(key);
    }

    public override string ToString()
    {
        return Key;
    }

    protected static IEnumerable<ConfigurationKeys> SupportedKeys
    {
        get
        {
            yield return new ConfigurationKeys(WebServiceUrl);
            yield return new ConfigurationKeys(AIServiceKey);
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Key;
    }
}
