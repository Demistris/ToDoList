using TodoList.Shared.Models;
using ToDoList.Shared.Models;

namespace ToDoListApi.Services
{
    public interface IUserService
    {
        Task<Response> RegisterUser(string username, string email, string password);
        Task<Response> AuthenticateUser(string email, string password);
        string GenerateJwtToken(User user);
    }
}
