using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public partial class ToDoList
    {
        private List<ToDoItem> _toDos = new List<ToDoItem>() { new ToDoItem { Description = "Test todo item", Completed = false }, new ToDoItem { Description = "Second todo item", Completed = true } };
        private ToDoItem _newToDoItem = new() { Description = "", Completed = false };

        private void AddNewItem()
        {
            if(_newToDoItem.Description != null)
            {
                _toDos.Add(new ToDoItem { Description = _newToDoItem.Description, Completed = false });
                _newToDoItem = new() { Description = "", Completed = false };
            }
        }

        private string GetTextDecorationStyle(ToDoItem toDoItem)
        {
            return toDoItem.Completed ? "text-decoration: line-through;" : "text-decoration: none;";
        }
    }
}
