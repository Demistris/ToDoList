using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TodoList.Shared.DTOs;
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
        private readonly IToDoItemService _toDoItemService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ToDoListController> _logger;

        public ToDoListController(IToDoListService toDoListService, IToDoItemService toDoItemService, IHttpContextAccessor httpContextAccessor, ILogger<ToDoListController> logger)
        {
            _toDoListService = toDoListService;
            _toDoItemService = toDoItemService;
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

        #region ToDoLists

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

        #endregion
        #region ToDos

        [HttpGet("{listId}/todos")]
        public async Task<ActionResult<List<ToDoItem>>> GetAllToDosForList(string listId)
        {
            try
            {
                var toDos = await _toDoItemService.GetListToDosAsync(listId);
                return Ok(toDos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all to dos");
                return StatusCode(500, new { Message = "An error occurred while getting all to dos", Error = ex.Message });
            }
        }

        [HttpPost("{listId}/todos")]
        public async Task<ActionResult<ToDoItem>> CreateToDo(string listId, [FromBody] CreateToDoItemDto newToDoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (newToDoDto.ToDoListModelId != listId)
            {
                return BadRequest("ListId mismatch between URL and request body.");
            }

            var newToDo = new ToDoItem
            {
                Description = newToDoDto.Description,
                Completed = newToDoDto.Completed,
                ToDoListModelId = newToDoDto.ToDoListModelId
            };

            try
            {
                var createdToDo = await _toDoItemService.AddToDoAsync(listId, newToDo);
                return CreatedAtAction(nameof(GetToDoById), new { id = createdToDo.Id }, createdToDo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the to do", Error = ex.Message });
            }
        }

        [HttpGet("todos/{id}")]
        public async Task<ActionResult<ToDoListModel>> GetToDoById(string id)
        {
            try
            {
                var toDo = await _toDoItemService.GetToDoByIdAsync(id);

                if (toDo == null)
                {
                    return NotFound();
                }

                return Ok(toDo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the list", Error = ex.Message });
            }
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateToDo(string id, ToDoItem updatedToDo)
        //{
        //    if (id != updatedToDo.Id)
        //    {
        //        return BadRequest("To do ID mismatch");
        //    }

        //    try
        //    {
        //        await _toDoItemService.UpdateToDoAsync(updatedToDo);
        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = "An error occurred while updating the to do", Error = ex.Message });
        //    }
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteToDo(string id)
        //{
        //    try
        //    {
        //        await _toDoItemService.DeleteToDoAsync(id);
        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = "An error occurred while deleting the to do", Error = ex.Message });
        //    }
        //}

        #endregion
    }
}
