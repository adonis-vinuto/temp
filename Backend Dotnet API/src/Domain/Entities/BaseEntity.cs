using Domain.Enums;
using Domain.Helpers;

namespace Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTimeProvider.DataHoraAtual();
}