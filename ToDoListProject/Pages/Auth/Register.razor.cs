using System.Text.Json;
using ToDoList.Shared.Models;

namespace ToDoListProject.Pages.Auth
{
    public partial class Register
    {
        private RegisterModel _registerModel = new RegisterModel();

        private async Task HandleRegistration()
        {
            try
            {
                // TODO: When Username is taken then show that message and do not try to register
                var user = await ApiService.RegisterUser(_registerModel);
                Navigation.NavigateTo("/");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Registration failed: {ex.Message}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing response: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
