using System.ComponentModel.DataAnnotations;

namespace ToDoListProject.Pages.Auth
{
    public partial class Login
    {
        private LoginModel _loginModel = new LoginModel();

        private async Task HandleLogin()
        {
            // TODO: Implement login logic
            Navigation.NavigateTo("/");
        }
    }

    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
