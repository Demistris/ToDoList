using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public partial class SortableList
    {
        [Parameter, AllowNull]
        public List<ToDoItem> Items
        {
            get => ToDoList._uncompletedToDoItems;
            set => ToDoList._uncompletedToDoItems = value;
        }

        [Parameter]
        public RenderFragment<ToDoItem>? SortableItem { get; set; }

        private DotNetObjectReference<SortableList>? _selfReference;
        [Inject]
        private IJSRuntime JS { get; set; }
        [Inject]
        private ToDoList ToDoList { get; set; }

        protected override void OnInitialized()
        {
            ToDoList.OnChange += StateHasChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                _selfReference = DotNetObjectReference.Create(this);
                await JS.InvokeAsync<object>("THEURLLIST.sortable.init", "sortableList", _selfReference);
            }
        }

        [JSInvokable]
        public void Drop(int oldIndex, int newIndex)
        {
            ToDoList.ReorderToDos(oldIndex, newIndex);
        }
    }
}