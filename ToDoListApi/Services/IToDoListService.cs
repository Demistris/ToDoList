using ToDoList.Shared.Models;

namespace ToDoListApi.Services
{
    public interface IToDoListService
    {
        Task<List<ToDoListModel>> GetUserListsAsync(int userId);
        Task<ToDoListModel> GetListByIdAsync(string listId, int userId);
        Task<ToDoListModel> AddListAsync(int userId, ToDoListModel newList);
        Task UpdateListAsync(ToDoListModel updatedList);
        Task DeleteListAsync(string listId, int userId);
    }
}
