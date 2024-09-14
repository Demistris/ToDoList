using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ToDoListApi.Database;
using ToDoList.Shared.Models;
using ToDoList.Shared.CustomExceptions;

namespace ToDoListApi.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterUser(string username, string email, string password)
        {
            if(await _context.Users.AnyAsync(u => u.Email == email))
            {
                throw new EmailAlreadyExistsException();
            }

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> AuthenticateUser(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.ToDoLists)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return storedHash == HashPassword(password);
        }
    }
}
