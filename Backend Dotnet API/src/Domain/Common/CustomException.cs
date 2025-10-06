using System;
using System.Collections.Generic;

namespace Domain.Common;

public class CustomException : Exception
{
    public Dictionary<string, object> AdditionalData { get; }

    public CustomException(string message) : base(message)
    {
        AdditionalData = [];
    }

    public CustomException(string message, Exception innerException) : base(message, innerException)
    {
        AdditionalData = [];
    }

    public CustomException WithData(string key, object value)
    {
        AdditionalData[key] = value;
        return this;
    }
}
