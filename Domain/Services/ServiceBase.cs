using AutoMapper;
using Domain.Interfaces;

namespace Domain.Services
{
    public abstract class ServiceBase<TEntity, TDto> where TEntity : class
    {
        private readonly IRepositoryBase<TEntity> _repository;
        protected readonly IMapper _mapper;

        protected ServiceBase(IRepositoryBase<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        public async Task<TDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<TDto>(entity);
        }

        public async Task<TDto> AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            var addedEntity = await _repository.AddAsync(entity);
            return _mapper.Map<TDto>(addedEntity);
        }
        public async Task<TDto> AddAsync(object entity)
        {
            var addedEntity = await _repository.AddAsync((TEntity)entity);
            return _mapper.Map<TDto>(addedEntity);
        }

        public async Task UpdateAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _repository.UpdateAsync(entity);
        }

        public async Task UpdateAsync(object entity)
        {
            await _repository.UpdateAsync((TEntity)entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(id);
            }
        }
    }

}
