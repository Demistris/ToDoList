using Microsoft.EntityFrameworkCore;
using ToDoList.Shared.Models;
using ToDoListApi.Database;

namespace ToDoListApi.Services
{
    public class ToDoListService : IToDoListService
    {
        private readonly ApplicationDbContext _context;

        public ToDoListService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ToDoListModel>> GetUserListsAsync(int userId)
        {
            return await _context.ToDoLists.Where(list => list.UserId == userId).Include(list => list.Items).ToListAsync();
        }

        public async Task<ToDoListModel> GetListByIdAsync(string listId, int userId)
        {
            return await _context.ToDoLists.FirstOrDefaultAsync(list => list.Id == listId && list.UserId == userId);
        }

        public async Task<ToDoListModel> AddListAsync(int userId, ToDoListModel newList)
        {
            newList.UserId = userId;
            _context.ToDoLists.Add(newList);
            await _context.SaveChangesAsync();

            return newList;
        }

        public async Task UpdateListAsync(ToDoListModel updatedList)
        {
            _context.ToDoLists.Update(updatedList);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteListAsync(string listId, int userId)
        {
            var list = await _context.ToDoLists.FirstOrDefaultAsync(list => list.Id == listId && list.UserId == userId);

            if(list != null)
            {
                _context.ToDoLists.Remove(list);
                await _context.SaveChangesAsync();
            }
        }
    }
}
