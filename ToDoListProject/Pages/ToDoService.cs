using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public class ToDoService
    {
        public List<ToDoListModel> GetAllLists() => _toDoLists;

        private List<ToDoListModel> _toDoLists = new List<ToDoListModel>();
        private bool _isEditing;

        public ToDoListModel AddList(string name)
        {
            //if (_toDoLists.Any(l => l.ListName == name))
            //{
            //    return null;
            //}

            var newList = new ToDoListModel { ListName = $"{name}{_toDoLists.Count + 1}" };
            _toDoLists.Add(newList);
            return newList;
        }

        public void DeleteList(string listId)
        {
            _toDoLists.RemoveAll(l => l.Id == listId);
        }

        public void UpdateList(ToDoListModel updatedList)
        {
            var index = _toDoLists.FindIndex(l => l.Id == updatedList.Id);

            if (index != -1)
            {
                _toDoLists[index] = updatedList;
            }
        }

        public ToDoListModel GetList(string listId)
        {
            return _toDoLists.FirstOrDefault(l => l.Id == listId);
        }

        private void EditListName()
        {
            _isEditing = true;
        }
    }
}
