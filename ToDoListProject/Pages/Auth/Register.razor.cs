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
                var user = await ApiService.RegisterUser(_registerModel);
                Navigation.NavigateTo("/login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration failed: {ex.Message}");
            }
        }
    }
}
