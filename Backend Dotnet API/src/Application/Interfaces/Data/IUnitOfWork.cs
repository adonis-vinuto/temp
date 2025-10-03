namespace Application.Interfaces.Data;

public interface IUnitOfWork
{
    Task Commit();
}