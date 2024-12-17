namespace Domain.RepositoryInterfaces;

public interface IUnitOfWork
{
    Task<int> Commit(CancellationToken cancellationToken=default);
    Task<int> DetachAllEntries(CancellationToken cancellationToken=default);
}