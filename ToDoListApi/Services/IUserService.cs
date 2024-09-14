using ToDoList.Shared.Models;

namespace ToDoListApi.Services
{
    public interface IUserService
    {
        Task<User> RegisterUser(string username, string email, string password);
        Task<User> AuthenticateUser(string email, string password);
    }
}
