using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public partial class ToDoItemComponent
    {
        [Parameter]
        public ToDoItem Item { get; set; }
        [Parameter]
        public EventCallback<ToDoItem> OnUpdate { get; set; }
        [Parameter]
        public EventCallback<ToDoItem> OnDelete { get; set; }

        private bool _isEditing;

        private async Task HandleCheckboxChange(ChangeEventArgs e)
        {
            if (Item != null)
            {
                Item.Completed = !Item.Completed;
                await OnUpdate.InvokeAsync(Item);
                StateHasChanged();
            }
        }

        private string GetTextDecorationStyle()
        {
            return Item.Completed ? "text-decoration: line-through;" : "text-decoration: none;";
        }

        private void EditDescription()
        {
            _isEditing = true;
            //editDescription = Item.Description;
        }

        private void HandleKeyDownToSaveEdit(KeyboardEventArgs e)
        {
            if(e.Key == "Enter")
            {
                SaveEdit();
            }
            else if(e.Key == "Escape")
            {
                _isEditing = false;
            }
        }

        private void SaveEdit()
        {
            //if (!string.IsNullOrWhiteSpace(editDescription))
            //{
            //    Item.Description = editDescription;
            //    OnUpdate.InvokeAsync(Item);
            //}

            _isEditing = false;
        }

        private async Task DeleteItem()
        {
            await OnDelete.InvokeAsync(Item);
            StateHasChanged();
        }
    }
}
