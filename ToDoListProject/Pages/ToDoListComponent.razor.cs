using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ToDoListProject.Models;
using ToDoListProject.Services;

namespace ToDoListProject.Pages
{
    public partial class ToDoListComponent
    {
        [Parameter] 
        public string ListId { get; set; }
        [Parameter] 
        public ToDoListModel ToDoListModel { get; set; }
        public event EventHandler ListNameChanged;

        private List<ToDoItem> _uncompletedToDoItems = [];
        private List<ToDoItem> _completedToDoItems = [];
        private ToDoItem _newToDoItem = new() { Description = "", Completed = false };
        private bool _isEditing;
        private string _editListName;
        private bool _showDeleteConfirmation = false;
        private int _maxListNameLength = 100;
        private int _maxToDoDescriptionLength = 200;
        private ElementReference _editInputElement;

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

        private void TrimString(string stringToTrim, int maxLength)
        {
            if (stringToTrim.Length > maxLength)
            {
                stringToTrim = stringToTrim.Length <= maxLength ? stringToTrim : stringToTrim[..maxLength];
            }
        }

        #region ToDosManagment

        private async Task HandleEnterKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !e.ShiftKey)
            {
                await Task.Delay(1);
                _newToDoItem.Description = _newToDoItem.Description.TrimEnd('\n', '\r');
                AddNewItem();
            }
        }

        private void AddNewItem()
        {
            if (string.IsNullOrWhiteSpace(_newToDoItem.Description))
            {
                return;
            }

            TrimString(_newToDoItem.Description, _maxToDoDescriptionLength);

            List<string> multilineText = _newToDoItem.Description.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(toDo => toDo.Trim())
                .Where(toDo => !string.IsNullOrWhiteSpace(toDo))
                .ToList();

            foreach (var toDo in multilineText)
            {
                var newItem = new ToDoItem
                {
                    Description = toDo,
                    Completed = false
                };

                if (ToDoListModel != null)
                {
                    ToDoListModel.Items.Add(newItem);
                    _uncompletedToDoItems.Add(newItem);
                    _newToDoItem.Description = string.Empty;
                    _ = OnUpdateListAsync();
                }
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

            _ = OnUpdateListAsync();
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

            _ = OnUpdateListAsync();
        }

        private void HandleReorder((int OldIndex, int NewIndex) reorderInfo)
        {
            var (oldIndex, newIndex) = reorderInfo;
            ReorderToDos(oldIndex, newIndex);
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

            _ = OnUpdateListAsync();
        }

        public int HowManyUncompletedToDos()
        {
            return _uncompletedToDoItems.Count;
        }

        #endregion
        #region ListManagment

        protected virtual void OnListNameChanged(EventArgs e)
        {
            ListNameChanged?.Invoke(this, e);
        }

        private async Task OnUpdateListAsync()
        {
            ToDoService.SetUncompletedCount(ListId, HowManyUncompletedToDos());
            await ToDoService.UpdateList(ToDoListModel);

            StateHasChanged();
        }
        
        private void EditListName()
        {
            _isEditing = true;
            _editListName = ToDoListModel.ListName;

            StateHasChanged();

            // Set focus after rendering the input
            _ = Task.Delay(1).ContinueWith(_ => _editInputElement.FocusAsync());
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

        private void SaveEdit()
        {
            if (!string.IsNullOrWhiteSpace(_editListName))
            {
                TrimString(_editListName, _maxListNameLength);

                ToDoListModel.ListName = _editListName;
                OnListNameChanged(EventArgs.Empty);
                _ = OnUpdateListAsync();
            }

            _isEditing = false;
        }

        private void DeleteList()
        {
            ToDoService.DeleteList(ListId);
            StateHasChanged();
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
