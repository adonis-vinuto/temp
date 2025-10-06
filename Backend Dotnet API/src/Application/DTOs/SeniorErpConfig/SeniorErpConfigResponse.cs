using Application.DTOs.Agent;
using System;

namespace Application.DTOs.SeniorErpConfig;

public class SeniorErpConfigResponse
{
    public Guid IdSeniorErpConfig { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? WsdlUrl { get; set; }
}