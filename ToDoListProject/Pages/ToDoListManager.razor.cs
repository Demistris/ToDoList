using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public partial class ToDoListManager
    {
        private ToDoListModel _selectedList;
        private string _newListName = "";

        private void SelectList(ToDoListModel list)
        {
            Console.WriteLine($"Selecting list with ID: {list?.Id}");
            _selectedList = list;
            StateHasChanged();
        }

        private void AddList()
        {
            if (!string.IsNullOrWhiteSpace(_newListName))
            {
                var newList = ToDoService.AddList(_newListName);
                _newListName = "";
                if (newList != null)
                {
                    SelectList(newList);
                }
                StateHasChanged();
            }
        }

        private void DeleteList(ToDoListModel list)
        {
            ToDoService.DeleteList(list.Id);

            if (_selectedList?.Id == list.Id)
            {
                _selectedList = null;
            }

            StateHasChanged();
        }

        private void UpdateList(ToDoListModel list)
        {
            ToDoService.UpdateList(list);
            StateHasChanged();
        }
    }
}
