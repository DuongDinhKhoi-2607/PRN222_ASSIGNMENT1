using BussinessLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLayer.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(long id);
        Task<UserDto?> AuthenticateAsync(string email, string password);
        Task<bool> EmailExistsAsync(string email);
        Task<UserDto> RegisterUserAsync(RegisterDto dto);
        Task<IEnumerable<UserDto>> GetAllLecturersAsync();
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
    }
}
