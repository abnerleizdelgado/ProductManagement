using Domain.Entities;

namespace Domain.Interfaces
{

    namespace EmployeeManagement.Domain.Interfaces.Repositories
    {
        public interface IUserRepository : IRepositoryBase<User>
        {
            Task<User> GetByUserAsync(string user);
        }
    }
}