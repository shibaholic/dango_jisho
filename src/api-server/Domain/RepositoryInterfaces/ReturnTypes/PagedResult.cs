namespace Domain.RepositoryInterfaces.ReturnTypes;

public class PagedResult<T>
{
    public T Data { get; init; }
    public int TotalElements { get; init; }
}