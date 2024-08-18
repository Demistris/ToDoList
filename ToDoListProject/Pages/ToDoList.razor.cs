using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public partial class ToDoList : IDisposable
    {
        [Parameter]
        public string ListId { get; set; }
        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
        private ToDoListModel _listModel { get; set; }
        private List<ToDoItem> _uncompletedToDoItems = [];
        private List<ToDoItem> _completedToDoItems = [];
        private ToDoItem _newToDoItem = new() { Description = "", Completed = false };
        private string _currentListId;

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine($"ToDoList OnInitializedAsync: ListId = {ListId}");
            await LoadList();
        }

        protected override async Task OnParametersSetAsync()
        {
            Console.WriteLine($"ToDoList OnParametersSetAsync: ListId = {ListId}");
            await LoadList();
        }

        private async Task LoadList()
        {
            if (!string.IsNullOrEmpty(ListId))
            {
                _listModel = ListManager.GetListById(ListId);
                if (_listModel != null)
                {
                    _uncompletedToDoItems = _listModel.Items.Where(item => !item.Completed).ToList();
                    _completedToDoItems = _listModel.Items.Where(item => item.Completed).ToList();
                    Console.WriteLine($"List loaded: {_listModel.ListName}, Items: {_listModel.Items.Count}");
                }
                else
                {
                    Console.WriteLine($"List not found for ID: {ListId}");
                }
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            Console.WriteLine($"ToDoList OnAfterRender: firstRender = {firstRender}, ListId = {ListId}");
            base.OnAfterRender(firstRender);
        }

        public void Dispose()
        {
            Console.WriteLine($"ToDoList Dispose: ListId = {ListId}");
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

            NotifyStateChanged();
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

            if (_listModel != null)
            {
                _listModel.Items.Add(newItem);
                _uncompletedToDoItems.Add(newItem);
                _newToDoItem.Description = string.Empty;
            }

            NotifyStateChanged();
        }

        private void UpdateItemCompletionStatus(ToDoItem toDoItem)
        {
            if (toDoItem == null || _listModel == null)
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

            NotifyStateChanged();
        }

        private void HandleDeleteItem(ToDoItem toDoItem)
        {
            if (toDoItem == null || _listModel == null)
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

            _listModel.Items.Remove(toDoItem);

            NotifyStateChanged();
        }
    }
}
