using ErrorOr;
using FluentValidation;

namespace Application.Handlers;

public abstract class BaseHandler
{
    protected BaseHandler()
    {
    }

    protected static List<Error> Validate<TEntity, TValidator>(TEntity entity, TValidator validator)
        where TEntity : class
        where TValidator : AbstractValidator<TEntity>
    {
        FluentValidation.Results.ValidationResult validationResult = validator.Validate(entity);

        return validationResult.Errors
            .Select(erro => Error.Validation(code: erro.ErrorCode, description: erro.ErrorMessage,
                new Dictionary<string, object>()
                {
                    { "propertyName", erro.PropertyName }
                }))
            .ToList();
    }
}
