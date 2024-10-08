﻿using Microsoft.AspNetCore.Components.Authorization;
using ToDoList.Shared.Models;

namespace ToDoListProject.Pages.Auth
{
    public partial class Register
    {
        private RegisterModel _registerModel = new RegisterModel();
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

        private async Task HandleRegistration()
        {
            try
            {
                var user = await ApiService.RegisterUser(_registerModel);
                Navigation.NavigateTo("/");
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    _errorMessage = "The email address is already registered.";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                else
                {
                    _errorMessage = "An error occurred during signing up. Please try again.";
                    Console.WriteLine($"Registration failed: {ex.Message}");
                }
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
