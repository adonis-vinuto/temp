using Domain.Enums;
using Domain.Helpers;

namespace Domain.Entities;

public sealed record Log(
    Guid? IdUser,
    string? NameUser,
    string? UserEmail,
    string ChangedEntity,
    TypeProcess TypeProcess,
    string LastState,
    string NewState
)
{
  public Guid Id { get; init; } = Guid.NewGuid();
  public DateTime CreatedAt { get; set; } = DateTimeProvider.DataHoraAtual();
}