using System.Text.Json;
using ToDoList.Shared.Models;

namespace ToDoListProject.Pages.Auth
{
    public partial class Register
    {
        private RegisterModel _registerModel = new RegisterModel();
        private string _errorMessage;

        private async Task HandleRegistration()
        {
            try
            {
                var user = await ApiService.RegisterUser(_registerModel);
                Navigation.NavigateTo("/login");
            }
            catch (HttpRequestException ex)
            {
                _errorMessage = $"Registration failed: {ex.Message}";

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (JsonException ex)
            {
                _errorMessage = $"Error parsing response: {ex.Message}";
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }
    }
}
