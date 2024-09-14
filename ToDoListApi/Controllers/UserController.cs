using Microsoft.AspNetCore.Mvc;
using TodoList.Shared.Models;
using ToDoList.Shared.CustomExceptions;
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
        public async Task<ActionResult<Response>> Register(RegisterModel registerModel)
        {
            try
            {
                var user = await _userService.RegisterUser(registerModel.Username, registerModel.Email, registerModel.Password);
                return Ok(user);
            }
            catch (EmailAlreadyExistsException)
            {
                return Conflict(new { message = "Email is already registered" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginModel loginModel)
        {
            var user = await _userService.AuthenticateUser(loginModel.Email, loginModel.Password);

            if(user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            if (user.Success)
            {
                return Ok(user);
            }

            return BadRequest(user);
        }
    }
}
