using Microsoft.AspNetCore.Components.Authorization;
using ToDoList.Shared.Models;
using ToDoListProject.Provider;
using ToDoListProject.Services;

namespace ToDoListProject.Pages.Auth
{
    public partial class Login
    {
        private LoginModel _loginModel = new LoginModel();
        private bool _isPasswordVisible = false;
        private string _errorMessage = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await CheckIfAuthenticated();
        }

        private async Task CheckIfAuthenticated()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                Navigation.NavigateTo("/");
            }
        }

        private async Task HandleLogin()
        {
            _errorMessage = string.Empty;

            try
            {
                var loginResponse = await ApiService.LoginUser(_loginModel);
                await LocalStorageService.SetItemAsync<string>("JwtToken", loginResponse.Token);

                // Notify the authentication state provider of login
                var customAuthStateProvider = (CustomAuthStateProvider)AuthenticationStateProvider;
                customAuthStateProvider.NotifyUserAuthentication(loginResponse.Token);

                Navigation.NavigateTo("/");

            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _errorMessage = "Incorrect email or password.";
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                _errorMessage = "An error occurred during signing in. Please try again.";
            }
            catch (Exception ex)
            {
                _errorMessage = "An unexpected error occurred. Please try again later.";
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void TogglePasswordVisibility()
        {
            _isPasswordVisible = !_isPasswordVisible;
        }
    }
}
