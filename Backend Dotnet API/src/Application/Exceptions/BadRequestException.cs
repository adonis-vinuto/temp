using System;
using System.Collections.Generic;

namespace GemelliApi.Application.Exceptions;

public class BadRequestException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public BadRequestException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>
            {
                { "Erro", new[] { message } }
            };
    }

    public BadRequestException(Dictionary<string, string[]> errors) : base("Erro de validação")
    {
        Errors = errors;
    }
}

