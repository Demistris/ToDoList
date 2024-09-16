using System.Globalization;
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

            // Handle Unauthorized status
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException("Unauthorized", null, System.Net.HttpStatusCode.Unauthorized);
            }

            // Handle BadRequest
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new HttpRequestException("Bad Request", null, System.Net.HttpStatusCode.BadRequest);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Response>();
        }

        #region ToDoList Service

        public async Task<List<ToDoListModel>> GetAllToDoListsAsync()
        {
            var response = await _httpClient.GetAsync("api/todolist");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ToDoListModel>>();
        }

        public async Task<ToDoListModel> GetToDoListAsync(string listId)
        {
            return await _httpClient.GetFromJsonAsync<ToDoListModel>($"api/todolist/{listId}");
        }

        public async Task<ToDoListModel> AddToDoListAsync(ToDoListModel newList)
        {
            Console.WriteLine($"Sending new list: {System.Text.Json.JsonSerializer.Serialize(newList)}");

            var response = await _httpClient.PostAsJsonAsync("api/todolist", newList);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ToDoListModel>();
        }

        public async Task UpdateToDoListAsync(ToDoListModel updatedList)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/todolist/{updatedList.Id}", updatedList);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteToDoListAsync(string listId)
        {
            var response = await _httpClient.DeleteAsync($"api/todolist/{listId}");
            response.EnsureSuccessStatusCode();
        }

        #endregion
    }
}