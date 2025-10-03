using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.SeniorErpConfig.Create;

public record CreateSeniorErpConfigRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string WsdlUrl { get; set; } = string.Empty;
}
public class CreateSeniorErpConfigRequestValidator : AbstractValidator<CreateSeniorErpConfigRequest>
{

    public CreateSeniorErpConfigRequestValidator()
    {

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("O nome de usuário é obrigatório.")
            .MaximumLength(100).WithMessage("O nome de usuário não pode exceder 100 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.")
            .MaximumLength(100).WithMessage("A senha não pode exceder 100 caracteres.");
            
        RuleFor(x => x.WsdlUrl)
            .NotEmpty().WithMessage("A URL do WSDL é obrigatória.")
            .MaximumLength(200).WithMessage("A URL do WSDL não pode exceder 200 caracteres.");
        
    }
}


