using System.Net.Http.Json;
using TodoList.Shared.Models;
using ToDoList.Shared.Models;

namespace ToDoListProject.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7291/");
        }

        public async Task<Response> RegisterUser(RegisterModel registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/user/register", registerModel);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                throw new HttpRequestException(errorResponse["message"], null, response.StatusCode);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Response>();
        }

        public async Task<Response> LoginUser(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/user/login", loginModel);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException("Unauthorized", null, System.Net.HttpStatusCode.Unauthorized);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Response>();
        }
    }
}