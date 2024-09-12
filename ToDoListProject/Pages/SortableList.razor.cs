using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace ToDoListProject.Pages
{
    public partial class SortableList<TItem>
    {
        [Parameter, AllowNull]
        public List<TItem> Items { get; set; }
        [Parameter] 
        public RenderFragment<TItem> ItemTemplate { get; set; }
        [Parameter] 
        public EventCallback<(int OldIndex, int NewIndex)> OnReorder { get; set; }

        private DotNetObjectReference<SortableList<TItem>>? _selfReference;
        [Inject]
        private IJSRuntime JS { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                _selfReference = DotNetObjectReference.Create(this);
                await JS.InvokeAsync<object>("THEURLLIST.sortable.init", "sortableList", _selfReference);
            }
        }

        [JSInvokable]
        public async Task Drop(int oldIndex, int newIndex)
        {
            await OnReorder.InvokeAsync((oldIndex, newIndex));
        }
    }
}