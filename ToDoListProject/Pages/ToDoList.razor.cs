using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using ToDoListProject.Models;

namespace ToDoListProject.Pages
{
    public partial class ToDoList
    {
        private List<ToDoItem> _uncompletedToDoItems = new();
        private List<ToDoItem> _completedToDoItems = [];

        private ToDoItem _newToDoItem = new() { Description = "", Completed = false };

        protected override void OnInitialized()
        {
            _uncompletedToDoItems =
            [
                new ToDoItem { Id = 1, Description = "Task 1", Completed = false },
                new ToDoItem { Id = 2, Description = "Task 2", Completed = false }
            ];
            _completedToDoItems =
            [
                new ToDoItem { Id = 3, Description = "Task 3", Completed = true }
            ];
        }

        private void HandleSubmit()
        {
            AddNewItem();
        }

        private void AddNewItem()
        {
            if(string.IsNullOrWhiteSpace(_newToDoItem.Description))
            {
                return;
            }

            var newItem = new ToDoItem
            {
                Id = GenerateUniqueId(),
                Description = _newToDoItem.Description,
                Completed = false
            };

            _uncompletedToDoItems.Add(newItem);
            _newToDoItem.Description = string.Empty;
        }

        private int GenerateUniqueId()
        {
            // Check if either list is not empty before calling Max()
            var allItems = _uncompletedToDoItems.Concat(_completedToDoItems);
            if (allItems.Any())
            {
                // Get the maximum ID from the combined list of items
                return allItems.Max(x => x.Id) + 1;
            }

            // Return 1 if no items are present
            return 1;
        }

        private void UpdateItemCompletionStatus(ToDoItem toDoItem)
        {
            if(toDoItem == null)
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

            StateHasChanged();
        }
    }
}
