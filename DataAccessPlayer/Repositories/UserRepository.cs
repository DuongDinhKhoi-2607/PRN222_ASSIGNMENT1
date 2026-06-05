using DataAccessPlayer.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class UserRepository
    {
        private readonly RagchatbotDbContext _db;
        public UserRepository(RagchatbotDbContext db) { _db = db; }

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email.Trim());
        }

        public async Task<bool> AnyEmailAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email == email.Trim());
        }

        public async Task<User> AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.Users.OrderBy(u => u.FullName).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllByRoleAsync(string role)
        {
            return await _db.Users
                .Where(u => u.Role == role && u.IsActive == true)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }
    }
}
