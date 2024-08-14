using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public partial class ToDoList
    {
        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public List<ToDoItem> _uncompletedToDoItems = [];
        private List<ToDoItem> _completedToDoItems = [];

        private ToDoItem _newToDoItem = new() { Description = "", Completed = false };

        protected override void OnInitialized()
        {
            _uncompletedToDoItems =
            [
                new ToDoItem { Description = "Task 1", Completed = false },
                new ToDoItem { Description = "Task 2", Completed = false }
            ];
            _completedToDoItems =
            [
                new ToDoItem { Description = "Task 3", Completed = true }
            ];
        }

        public void ReorderToDos(int oldIndex, int newIndex)
        {
            Console.WriteLine($"_uncompletedToDoItems.Count: {_uncompletedToDoItems.Count}");

            var toDos = _uncompletedToDoItems;

            if (oldIndex < 0 || oldIndex >= toDos.Count || newIndex < 0 || newIndex >= toDos.Count)
            {
                Console.WriteLine($"Invalid indices: oldIndex={oldIndex}, newIndex={newIndex}");
                return;
            }

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

        private void HandleSubmit()
        {
            AddNewItem();
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

            _uncompletedToDoItems.Add(newItem);
            _newToDoItem.Description = string.Empty;
        }

        private void UpdateItemCompletionStatus(ToDoItem toDoItem)
        {
            if (toDoItem == null)
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
            if (toDoItem.Completed)
            {
                _completedToDoItems.Remove(toDoItem);
            }
            else
            {
                _uncompletedToDoItems.Remove(toDoItem);
            }

            NotifyStateChanged();
        }
    }
}
