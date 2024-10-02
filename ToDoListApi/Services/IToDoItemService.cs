using ToDoList.Shared.Models;

namespace ToDoListApi.Services
{
    public interface IToDoItemService
    {
        Task<List<ToDoItem>> GetListToDosAsync(string listId);
        Task<ToDoItem> AddToDoAsync(string listId, ToDoItem newToDo);
        Task UpdateToDoAsync(ToDoItem updatedToDo);
        Task DeleteToDoAsync(string toDoId);
        Task<ToDoItem> GetToDoByIdAsync(string toDoId);
    }
}
