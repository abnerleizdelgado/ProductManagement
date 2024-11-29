using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces.EmployeeManagement.Domain.Interfaces.Repositories;
using Domain.Interfaces;
using AutoMapper;

namespace Domain.Services
{
    public class UserService : ServiceBase<User, UserDTO>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IMapper mapper)
            : base(userRepository, mapper)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetByUserAsync(string username)
        {
            return await _userRepository.GetByUserAsync(username);
        }
    }
}



