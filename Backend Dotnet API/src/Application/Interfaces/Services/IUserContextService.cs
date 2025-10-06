namespace Application.Interfaces.Services;

public interface IUserContextService
{
  void SetUser(LogUser user);
  LogUser? GetUser();
}

public record LogUser
{
  public Guid Id { get; init; }
  public string? Email { get; init; }
  public string? Name { get; init; }
}
