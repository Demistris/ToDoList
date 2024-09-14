using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ToDoListApi.Database;
using ToDoList.Shared.Models;
using ToDoList.Shared.CustomExceptions;
using TodoList.Shared.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ToDoListApi.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Response> RegisterUser(string username, string email, string password)
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
            return new Response() { Success = true, Message = "Successfully created"};
        }

        public async Task<Response> AuthenticateUser(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.ToDoLists)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return new Response() { Success = false, Message = "Invalid email or password" };
            }

            var jwtToken = GenerateJwtToken(user);
            return new Response() { Success = true, Message = "Successfully logged", Token = jwtToken };
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

        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            claims.Add(new Claim("UserId", user.Id.ToString()));
            claims.Add(new Claim("Username", user.Username));
            claims.Add(new Claim("Email", user.Email));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
