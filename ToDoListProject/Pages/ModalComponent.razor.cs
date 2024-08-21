using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace ToDoListProject.Pages
{
    public partial class ModalComponent
    {
        [Parameter]
        public bool Show { get; set; }
        [Parameter]
        public string Title { get; set; }
        [Parameter]
        public string Content { get; set; }
        [Parameter]
        public EventCallback<MouseEventArgs> OnCancel { get; set; }
        [Parameter]
        public EventCallback<MouseEventArgs> OnAccept { get; set; }
    }
}
