using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public class ToDoService
    {
        public List<ToDoListModel> GetAllLists() => _toDoLists;

        private List<ToDoListModel> _toDoLists = new List<ToDoListModel>();

        public async Task<ToDoListModel> AddListAsync(string listName)
        {
            var newList = new ToDoListModel
            {
                ListName = listName,
                Items = new List<ToDoItem>()
            };

            _toDoLists.Add(newList);
            await Task.CompletedTask;
            return newList;
        }

        public void DeleteList(string listId)
        {
            _toDoLists.RemoveAll(l => l.Id == listId);
        }

        public async Task UpdateList(ToDoListModel updatedList)
        {
            var index = _toDoLists.FindIndex(l => l.Id == updatedList.Id);

            if (index != -1)
            {
                _toDoLists[index] = updatedList;
            }

            await Task.CompletedTask;
        }

        public ToDoListModel GetList(string listId)
        {
            return _toDoLists.FirstOrDefault(l => l.Id == listId);
        }
    }
}
