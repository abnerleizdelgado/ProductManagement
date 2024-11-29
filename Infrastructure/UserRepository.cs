using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.EmployeeManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User> GetByUserAsync(string user)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Username == user);
        }

    }
}
