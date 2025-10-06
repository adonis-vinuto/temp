using ErrorOr;

namespace Domain.Errors;

public static class OrganizationErrors
{
    public static readonly Error OrganizationNotFound = Error.NotFound(
        code: "Organization.NotFound",
        description: "Organização não encontrada.");
}