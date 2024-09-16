using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ToDoList.Shared.Models;
using ToDoListApi.Services;

namespace ToDoListApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToDoListController : Controller
    {
        private readonly IToDoListService _toDoListService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ToDoListController> _logger;

        public ToDoListController(IToDoListService toDoListService, IHttpContextAccessor httpContextAccessor, ILogger<ToDoListController> logger)
        {
            _toDoListService = toDoListService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("UserId");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Invalid user ID in token");
        }

        [HttpGet]
        public async Task<ActionResult<List<ToDoListModel>>> GetAllLists()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var lists = await _toDoListService.GetUserListsAsync(userId);
                return Ok(lists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all lists");
                return StatusCode(500, new { Message = "An error occurred while getting all lists", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoListModel>> GetListById(string id)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var list = await _toDoListService.GetListByIdAsync(id, userId);

                if (list == null)
                {
                    return NotFound();
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting list with id {id}");
                return StatusCode(500, new { Message = "An error occurred while retrieving the list", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ToDoListModel>> CreateList(ToDoListModel newList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (newList.UserId <= 0)
            {
                return BadRequest("Invalid UserId");
            }

            try
            {
                var userId = GetUserIdFromToken();
                if (userId != newList.UserId)
                {
                    return BadRequest("UserId mismatch");
                }

                _logger.LogInformation($"Creating list for user {userId}. List data: {JsonSerializer.Serialize(newList)}");
                var createdList = await _toDoListService.AddListAsync(userId, newList);
                return CreatedAtAction(nameof(GetListById), new { id = createdList.Id }, createdList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating list");
                return StatusCode(500, new { Message = "An error occurred while creating the list", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateList(string id, ToDoListModel updatedList)
        {
            if(id != updatedList.Id)
            {
                return BadRequest("List ID mismatch");
            }

            try
            {
                await _toDoListService.UpdateListAsync(updatedList);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating list with id {id}");
                return StatusCode(500, new { Message = "An error occurred while updating the list", Error = ex.Message });
            } 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteList(string id)
        {
            try
            {
                var userId = GetUserIdFromToken();
                await _toDoListService.DeleteListAsync(id, userId);
                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error deleting list with id {id}");
                return StatusCode(500, new { Message = "An error occurred while deleting the list", Error = ex.Message });
            }         
        }
    }
}
