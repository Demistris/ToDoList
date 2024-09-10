using System.ComponentModel.DataAnnotations;

namespace ToDoListProject.Pages.Auth
{
    public partial class Register
    {
        private RegisterModel _registerModel = new RegisterModel();

        private async Task HandleRegistration()
        {
            // TODO: Implement registration logic
            Navigation.NavigateTo("/login");
        }
    }

    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
