using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public partial class ToDoListManager
    {
        private static List<ToDoListModel> _toDoLists { get; set; } = new List<ToDoListModel>();
        private string _newListName = string.Empty;
        private ToDoListModel? _selectedList;


        //public ToDoListManager() 
        //{
        //    var homeList = new ToDoListModel
        //    {
        //        Id = Guid.NewGuid().ToString(),
        //        ListName = "Home",
        //        Items = new List<ToDoItem>
        //        {
        //            new ToDoItem { Description = "Clean the kitchen", Completed = false },
        //            new ToDoItem { Description = "Do the laundry", Completed = true }
        //        }
        //    };

        //    var workList = new ToDoListModel
        //    {
        //        Id = Guid.NewGuid().ToString(),
        //        ListName = "Work",
        //        Items = new List<ToDoItem>
        //        {   
        //            new ToDoItem { Description = "Complete project report", Completed = false },
        //            new ToDoItem { Description = "Email the client", Completed = true }
        //        }
        //    };

        //    _toDoLists.Add(homeList);
        //    _toDoLists.Add(workList);
        //}

        public ToDoListManager()
        {
            // Initialize with some sample data
            _toDoLists.Add(new ToDoListModel { Id = Guid.NewGuid().ToString(), ListName = "Home", Items = new List<ToDoItem>() });
            _toDoLists.Add(new ToDoListModel { Id = Guid.NewGuid().ToString(), ListName = "Work", Items = new List<ToDoItem>() });
        }
        public IReadOnlyList<ToDoListModel> ToDoLists => _toDoLists.AsReadOnly();

        private void InitializeLists()
        {
            var homeList = new ToDoListModel
            {
                Id = Guid.NewGuid().ToString(),
                ListName = "Home",
                Items = new List<ToDoItem>
            {
                new ToDoItem { Description = "Clean the kitchen", Completed = false },
                new ToDoItem { Description = "Do the laundry", Completed = true }
            }
            };
            var workList = new ToDoListModel
            {
                Id = Guid.NewGuid().ToString(),
                ListName = "Work",
                Items = new List<ToDoItem>
            {
                new ToDoItem { Description = "Complete project report", Completed = false },
                new ToDoItem { Description = "Email the client", Completed = true }
            }
            };

            _toDoLists.Add(homeList);
            _toDoLists.Add(workList);
        }


        public ToDoListModel GetListById(string id)
        {
            Console.WriteLine($"GetListById called: id = {id}");
            return _toDoLists.FirstOrDefault(l => l.Id == id);

            //return _toDoLists.FirstOrDefault(l => l.Id == id);
        }

        public void AddList(string listName)
        {
            _toDoLists.Add(new ToDoListModel { ListName = listName });
        }

        public void CreateNewList()
        {
            //after clicking this i want to add new input to list and focus on it's name to name list
            _newListName = "untitled";

            if (!string.IsNullOrWhiteSpace(_newListName))
            {
                AddList(_newListName);
                _newListName = string.Empty;
            }
        }

        private void SelectList(ToDoListModel list)
        {
            _selectedList = list;
        }
    }
}
