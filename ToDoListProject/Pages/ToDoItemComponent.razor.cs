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
        public EventCallback<ToDoItem> OnItemChanged { get; set; }
        [Parameter]
        public EventCallback<ToDoItem> OnDelete { get; set; }
        private bool _isEditing;

        private async Task HandleCheckboxChange(ChangeEventArgs e)
        {
            if (Item != null)
            {
                Item.Completed = (bool)e.Value;
                await OnItemChanged.InvokeAsync(Item);
                // Save to database
            }
        }

        private string GetTextDecorationStyle()
        {
            return Item.Completed ? "text-decoration: line-through;" : "text-decoration: none;";
        }

        private void EditDescription()
        {
            _isEditing = true;
        }

        private void HandleKeyDownToSaveEdit(KeyboardEventArgs e)
        {
            if(e.Key == "Enter")
            {
                SaveEdit();
            }
        }

        private void SaveEdit()
        {
            _isEditing = false;
            // Save to database
        }

        private async Task DeleteItem()
        {
            await OnDelete.InvokeAsync(Item);
        }
    }
}
