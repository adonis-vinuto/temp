using Microsoft.AspNetCore.Identity;

namespace Authentication.Models;

public class IdentityMensagensPortuguesConfig : IdentityErrorDescriber
{
    //podemos reescrever varios metodos aqui para mudar as mensagens do identity
    public override IdentityError DefaultError()
    {
        return new IdentityError
        {
            Code = nameof(DefaultError),
            Description = $"Ocorreu um erro desconhecido."
        };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = $"O E-mail {email} ja esta sendo usado."
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {

        return new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = $"O usuario {userName} ja esta sendo usado."
        };
    }

    public override IdentityError InvalidEmail(string? email)
    {
        return new IdentityError
        {
            Code = nameof(InvalidEmail),
            Description = $"O E-mail é invalido."
        };
    }

    public override IdentityError InvalidToken()
    {
        return new IdentityError
        {
            Code = nameof(InvalidToken),
            Description = "Token invalido."
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "A senha requer ao menos uma letra maiúcula"
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "A senha requer ao menos um caractere especial"
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "A senha requer ao menos um número"
        };
    }

    public override IdentityError PasswordMismatch()
    {
        return new IdentityError
        {
            Code = nameof(PasswordMismatch),
            Description = "A senha inválida"
        };
    }
}