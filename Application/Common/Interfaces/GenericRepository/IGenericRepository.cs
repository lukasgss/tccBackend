namespace Application.Common.Interfaces.GenericRepository;

public interface IGenericRepository<T>
{
    void Add(T entity);
    void AddRange(List<T> entities);
    void Update(T entity);
    void Delete(T entity);
    Task CommitAsync();
}