
using Domain.DTOs;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserService : IServiceBase<UserDTO>
    {
        Task<User> GetByUserAsync(string username);
    }
}
