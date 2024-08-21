using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        private bool _isEditing;
        private string _editListName;
        private bool _showDeleteConfirmation = false;

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

#region ToDosManagment

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

        #endregion
        #region ListManagment

        public event EventHandler ListNameChanged;

        protected virtual void OnListNameChanged(EventArgs e, string s)
        {
            ListNameChanged?.Invoke(s, e);
        }

        private void EditListName()
        {
            _isEditing = true;
            _editListName = ToDoListModel.ListName;
        }

        private void HandleKeyDownToSaveEdit(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                SaveEdit();
            }
            else if (e.Key == "Escape")
            {
                _isEditing = false;
            }
        }

        private async void SaveEdit()
        {
            if (!string.IsNullOrWhiteSpace(_editListName))
            {
                ToDoListModel.ListName = _editListName;
                OnListNameChanged(EventArgs.Empty, _editListName);
            }

            _isEditing = false;
            StateHasChanged();
        }

        private void DeleteList()
        {
            //await OnDelete.InvokeAsync();
            //StateHasChanged();
        }

        private void ShowDeleteConfirmation()
        {
            _showDeleteConfirmation = true;
        }

        private void AcceptDeleteConfirmation(MouseEventArgs e)
        {
            _showDeleteConfirmation = false;
            DeleteList();
        }

        private void HideDeleteConfirmation(MouseEventArgs e)
        {
            _showDeleteConfirmation = false;
        }

        #endregion
    }
}
