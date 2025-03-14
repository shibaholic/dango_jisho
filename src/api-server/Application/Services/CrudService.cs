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
    Task DeleteAsync(T entity);
    Task<Response<T>> GetByIdAsync(object id);
    Task<Response<TDto>> GetDtoByIdAsync(object id);
    Task<Response<IEnumerable<TDto>>> GetAllDtoAsync();
    Task<Response<IEnumerable<T>>> GetAllAsync(int? take = null);
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
        await _repo.CreateAsync(entity);
        return Response<TDto>.NoContent();
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

    public async Task DeleteAsync(T entity)
    {
        await _repo.DeleteAsync(entity);
    }

    public async Task<Response<TDto>> GetDtoByIdAsync(object id)
    {
        var entity = await _repo.ReadByIdAsync(id);
        if (entity == null) return Response<TDto>.NotFound("Entity not found");
        var dto = _mapper.Map<TDto>(entity);
        return Response<TDto>.Ok("Entity dto found", dto);
    }
    
    public async Task<Response<T>> GetByIdAsync(object id)
    {
        var entity = await _repo.ReadByIdAsync(id);
        if (entity == null) return Response<T>.NotFound("Entity not found");
        return Response<T>.Ok("Entity found", entity);
    }

    public async Task<Response<IEnumerable<TDto>>> GetAllDtoAsync()
    {
        var result = await _repo.ReadAllAsync();
        var dto = _mapper.Map<IEnumerable<TDto>>(result);
        return Response<IEnumerable<TDto>>.Ok("Entity dto results", dto);
    }

    public async Task<Response<IEnumerable<T>>> GetAllAsync(int? take = null)
    {
        var result = await _repo.ReadAllAsync(take);
        return Response<IEnumerable<T>>.Ok("Entity results", result);
    }
}