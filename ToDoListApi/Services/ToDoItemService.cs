using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ToDoList.Shared.Models;
using ToDoListApi.Database;

namespace ToDoListApi.Services
{
    public class ToDoItemService : IToDoItemService
    {
        private readonly ApplicationDbContext _context;

        public ToDoItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ToDoItem>> GetListToDosAsync(string listId)
        {
            return await _context.ToDoItems.Where(toDo => toDo.ToDoListModelId == listId).Include(toDo => toDo.ToDoListModelId).ToListAsync();
        }

        public async Task<ToDoItem> AddToDoAsync(string listId, ToDoItem newToDo)
        {
            var list = await _context.ToDoLists.Include(l => l.Items).FirstOrDefaultAsync(l => l.Id == listId);

            if (list == null)
            {
                throw new KeyNotFoundException($"List with id {listId} not found.");
            }

            newToDo.ToDoListModelId = listId;
            _context.ToDoItems.Add(newToDo);
            await _context.SaveChangesAsync();

            return newToDo;
        }

        public async Task<ToDoItem> GetToDoByIdAsync(string toDoId)
        {
            return await _context.ToDoItems.FirstOrDefaultAsync(list => list.Id == toDoId);
        }

        public async Task UpdateToDoAsync(ToDoItem updatedToDo)
        {
            _context.ToDoItems.Update(updatedToDo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteToDoAsync(string toDoId)
        {
            var toDo = await _context.ToDoItems.FirstOrDefaultAsync(t => t.Id == toDoId);

            if (toDo != null)
            {
                _context.ToDoItems.Remove(toDo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
