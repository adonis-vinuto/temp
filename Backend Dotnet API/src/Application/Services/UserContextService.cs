using Application.Interfaces.Services;

namespace Application.Services;

public class UserContextService : IUserContextService
{
  private LogUser? _user;

  public void SetUser(LogUser user) => _user = user;
  public LogUser? GetUser() => _user;
}
