namespace ToDoListProject.Pages
{
    public partial class ToDoList
    {
        private List<string> _toDoList = new List<string>() { "qwe" };
        private string _newToDoItem = "";

        private void AddNewItem()
        {
            _toDoList.Add(_newToDoItem);
            _newToDoItem = "";
        }
    }
}
