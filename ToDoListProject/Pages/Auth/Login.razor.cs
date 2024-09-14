using ToDoList.Shared.Models;

namespace ToDoListProject.Pages.Auth
{
    public partial class Login
    {
        private LoginModel _loginModel = new LoginModel();
        private bool _isPasswordVisible = false;

        private async Task HandleLogin()
        {
            // TODO: Add error messages when incorrect username or password
            try
            {
                var user = await ApiService.LoginUser(_loginModel);
                Navigation.NavigateTo("/");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void TogglePasswordVisibility()
        {
            _isPasswordVisible = !_isPasswordVisible;
        }
    }
}
