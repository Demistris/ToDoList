using ToDoList.Shared.Models;

namespace ToDoListProject.Pages.Auth
{
    public partial class Login
    {
        private LoginModel _loginModel = new LoginModel();
        private bool _isPasswordVisible = false;
        private string _errorMessage = string.Empty;

        private async Task HandleLogin()
        {
            _errorMessage = string.Empty;

            try
            {
                var user = await ApiService.LoginUser(_loginModel);
                Navigation.NavigateTo("/");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _errorMessage = "Incorrect email or password.";
            }
            catch (Exception ex)
            {
                _errorMessage = "An error occurred. Please try again.";
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void TogglePasswordVisibility()
        {
            _isPasswordVisible = !_isPasswordVisible;
        }
    }
}
