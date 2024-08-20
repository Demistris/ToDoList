using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public partial class ToDoListManager
    {
        private ToDoListModel _selectedList;
        private string _newListName = "";

        private async Task SelectListAsync(ToDoListModel list)
        {
            _selectedList = list;
            StateHasChanged();
            await Task.CompletedTask;
        }

        public async Task<ToDoListModel> AddList()
        {
            _newListName = "Untitled";

            if (!string.IsNullOrWhiteSpace(_newListName))
            {
                var newList = await ToDoService.AddListAsync(_newListName);
                _newListName = "";

                if (newList != null)
                {
                    await SelectListAsync(newList);
                }

                StateHasChanged();
                return newList;
            }

            return null;
        }

        private void DeleteList(ToDoListModel list)
        {
            ToDoService.DeleteList(list.Id);
            //NavigationManager.NavigateTo("/lists", forceLoad: true);

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
