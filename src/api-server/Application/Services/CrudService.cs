using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using EntityFramework.Exceptions.Common;
using Application.Response;
using AutoMapper;

namespace Application.Services;

public interface ICrudService<T, TDto>
{
    Task<Response<TDto>> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
}

public class CrudService<T, TDto> : ICrudService<T, TDto> where T: IBaseEntity
{
    private readonly IBaseRepository<T> _repo;
    private readonly IMapper _mapper;

    public CrudService(IBaseRepository<T> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Response<TDto>> CreateAsync(T entity)
    {
        var result = await _repo.CreateAsync(entity);
        var dto = _mapper.Map<TDto>(result);
        return Response<TDto>.Ok("Entity added", dto);
    }

    public async Task<Response<IEnumerable<TDto>>> CreateRangeAsync(IEnumerable<T> entities)
    {
        var result = await _repo.CreateRangeAsync(entities);
        var dto = _mapper.Map<IEnumerable<TDto>>(result);
        return Response<IEnumerable<TDto>>.Ok("Entity added", dto);
    }

    public async Task<T> UpdateAsync(T entity)
    {
        return await _repo.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repo.DeleteAsync(id);
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _repo.ReadByIdAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repo.ReadAllAsync();
    }
}