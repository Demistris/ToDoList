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
            if(!String.IsNullOrEmpty(_newToDoItem.Description))
            {
                _uncompletedToDoItems.Add(new ToDoItem { Description = _newToDoItem.Description, Completed = false });
                _newToDoItem = new() { Description = "", Completed = false };
            }
        }

        private void UpdateItemCompletionStatus(ToDoItem toDoItem)
        {
            if(toDoItem != null)
            {
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
}
