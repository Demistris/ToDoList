using ToDoList.Shared.Models;

namespace ToDoListProject.Pages.Auth
{
    public partial class Login
    {
        private LoginModel _loginModel = new LoginModel();

        private async Task HandleLogin()
        {
            var user = await ApiService.LoginUser(_loginModel);

            if(user != null)
            {
                Navigation.NavigateTo("/");
            }
            else
            {
                // TODO: Handle login failure (e.g., display error message)
            }
        }
    }
}
