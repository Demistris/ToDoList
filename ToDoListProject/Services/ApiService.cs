using System.Net.Http.Json;
using ToDoList.Shared.Models;

namespace ToDoListProject.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:44398/"); // Adjust this to match your API's URL
        }

        public async Task<User> RegisterUser(RegisterModel registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/user/register", registerModel);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<User>();
        }
        
        public async Task<User> LoginUser(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/user/login", loginModel);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<User>();
        }
    }
}