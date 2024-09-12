using Microsoft.AspNetCore.Mvc;
using ToDoList.Shared.Models;
using ToDoListApi.Services;

namespace ToDoListApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            try
            {
                var user = await _userService.RegisterUser(registerModel.Username, registerModel.Email, registerModel.Password);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var user = await _userService.AuthenticateUser(loginModel.Username, loginModel.Password);

            if(user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            //TODO: Generate and return a JWT token
            return Ok(user);
        }
    }
}
