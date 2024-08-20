using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public partial class ToDoListComponent
    {
        [Parameter] 
        public string ListId { get; set; }
        [Parameter] 
        public ToDoListModel ToDoListModel { get; set; }
        [Parameter] 
        public EventCallback<ToDoListModel> OnListUpdate { get; set; }

        private List<ToDoItem> _uncompletedToDoItems = [];
        private List<ToDoItem> _completedToDoItems = [];
        private ToDoItem _newToDoItem = new() { Description = "", Completed = false };

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrEmpty(ListId))
            {
                ToDoListModel = ToDoService.GetList(ListId);

                if (ToDoListModel != null)
                {
                    _uncompletedToDoItems = ToDoListModel.Items.Where(i => !i.Completed).ToList();
                    _completedToDoItems = ToDoListModel.Items.Where(i => i.Completed).ToList();
                }
            }
        }

        private void AddNewItem()
        {
            if (string.IsNullOrWhiteSpace(_newToDoItem.Description))
            {
                return;
            }

            var newItem = new ToDoItem
            {
                Description = _newToDoItem.Description,
                Completed = false
            };

            if (ToDoListModel != null)
            {
                ToDoListModel.Items.Add(newItem);
                _uncompletedToDoItems.Add(newItem);
                _newToDoItem.Description = string.Empty;
                OnListUpdate.InvokeAsync(ToDoListModel);
                StateHasChanged();
            }
        }

        private void UpdateItemCompletionStatus(ToDoItem toDoItem)
        {
            if (toDoItem == null || ToDoListModel == null)
            {
                return;
            }

            if (toDoItem.Completed)
            {
                if (_uncompletedToDoItems.Contains(toDoItem))
                {
                    _uncompletedToDoItems.Remove(toDoItem);
                }
                if (!_completedToDoItems.Contains(toDoItem))
                {
                    _completedToDoItems.Insert(0, toDoItem);
                }
            }
            else
            {
                if (_completedToDoItems.Contains(toDoItem))
                {
                    _completedToDoItems.Remove(toDoItem);
                }
                if (!_uncompletedToDoItems.Contains(toDoItem))
                {
                    _uncompletedToDoItems.Insert(0, toDoItem);
                }
            }

            OnListUpdate.InvokeAsync(ToDoListModel);
            StateHasChanged();
        }

        public void ReorderToDos(int oldIndex, int newIndex)
        {
            var toDos = _uncompletedToDoItems;
            var itemToMove = toDos[oldIndex];
            toDos.RemoveAt(oldIndex);

            if (oldIndex < toDos.Count)
            {
                toDos.Insert(newIndex, itemToMove);
            }
            else
            {
                toDos.Add(itemToMove);
            }

            OnListUpdate.InvokeAsync(ToDoListModel);
            StateHasChanged();
        }

        private void HandleReorder((int OldIndex, int NewIndex) reorderInfo)
        {
            var (oldIndex, newIndex) = reorderInfo;
            ReorderToDos(oldIndex, newIndex);
        }

        private void HandleDeleteItem(ToDoItem toDoItem)
        {
            if (toDoItem == null || ToDoListModel == null)
            {
                return;
            }

            if (toDoItem.Completed)
            {
                _completedToDoItems.Remove(toDoItem);
            }
            else
            {
                _uncompletedToDoItems.Remove(toDoItem);
            }

            ToDoListModel.Items.Remove(toDoItem);

            OnListUpdate.InvokeAsync(ToDoListModel);
            StateHasChanged();
        }

    }
}
