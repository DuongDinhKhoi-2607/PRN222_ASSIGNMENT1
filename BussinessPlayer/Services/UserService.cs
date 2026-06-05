using DataAccessLayer.Repositories;
using DataAccessPlayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using BussinessLayer.Interfaces;
using BussinessLayer.DTOs;

namespace BussinessLayer.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepo;

        public UserService(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        private static UserDto MapToDto(User u) => new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt,
            IsActive = u.IsActive
        };

        public async Task<UserDto?> GetByIdAsync(long id)
        {
            var u = await _userRepo.GetByIdAsync(id);
            return u == null ? null : MapToDto(u);
        }

        public async Task<UserDto?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null || !user.IsActive.GetValueOrDefault(true))
                return null;

            bool isValid = PasswordHelper.VerifyPassword(password, user.PasswordHash);
            if (!isValid)
                return null;

            return MapToDto(user);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepo.AnyEmailAsync(email);
        }

        public async Task<UserDto> RegisterUserAsync(RegisterDto dto)
        {
            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                Role = dto.Role.Trim().ToLower(),
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var created = await _userRepo.AddAsync(user);
            return MapToDto(created);
        }

        public async Task<IEnumerable<UserDto>> GetAllLecturersAsync()
        {
            var users = await _userRepo.GetAllByRoleAsync("lecturer");
            return users.Select(MapToDto);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return users.Select(MapToDto);
        }
    }
}
