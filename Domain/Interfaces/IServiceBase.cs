namespace Domain.Interfaces
{
    public interface IServiceBase<TDto>
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(int id);
        Task<TDto> AddAsync(TDto dto);
        Task<TDto> AddAsync(object entity);
        Task UpdateAsync(TDto dto);
        Task UpdateAsync(object entity);
        Task DeleteAsync(int id);
    }
}
