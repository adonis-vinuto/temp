using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.SeniorHcmConfig;

public class SeniorHcmConfigResponse
{
    public Guid IdSeniorHcmConfig { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? WsdlUrl { get; set; }
}
