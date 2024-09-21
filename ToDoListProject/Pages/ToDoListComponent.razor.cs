using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using ToDoList.Shared.Models;
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
        private bool _shouldFocusInput = false;
        private bool _preventDefault;
        private bool _isLoading = true;

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(ListId))
            {
                ToDoListModel = await ApiService.GetToDoListAsync(ListId);

                if (ToDoListModel != null)
                {
                    _uncompletedToDoItems = ToDoListModel.Items.Where(i => !i.Completed).ToList();
                    _completedToDoItems = ToDoListModel.Items.Where(i => i.Completed).ToList();
                }

                await LoadTodoList();
            }
        }

        private async Task LoadTodoList()
        {
            _isLoading = true;

            try
            {
                ToDoListModel = await Http.GetFromJsonAsync<ToDoListModel>($"api/todolist/{ListId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading todo list: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
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
        
        private void HandleEnterKeyDown(KeyboardEventArgs e)
        {
            _preventDefault = e.Key == "Enter" && !e.ShiftKey;

            if (e.Key == "Enter" && !e.ShiftKey)
            {
                AddNewItem();
            }
        }

        private async void AddNewItem()
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
                    await OnUpdateListAsync();
                }
            }
        }

        private async void UpdateItemCompletionStatus(ToDoItem toDoItem)
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

            await OnUpdateListAsync();
        }

        private async void HandleDeleteItem(ToDoItem toDoItem)
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

            await OnUpdateListAsync();
        }

        private void HandleReorder((int OldIndex, int NewIndex) reorderInfo)
        {
            var (oldIndex, newIndex) = reorderInfo;
            ReorderToDos(oldIndex, newIndex);
        }

        public async void ReorderToDos(int oldIndex, int newIndex)
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

            await OnUpdateListAsync();
        }

        public int HowManyUncompletedToDos()
        {
            return _uncompletedToDoItems.Count;
        }

        #endregion
        #region ListManagment

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_shouldFocusInput)
            {
                _shouldFocusInput = false;
                await _editInputElement.FocusAsync();
            }
        }

        protected virtual void OnListNameChanged(EventArgs e)
        {
            ListNameChanged?.Invoke(this, e);
        }

        private async Task OnUpdateListAsync()
        {
            ToDoService.SetUncompletedCount(ListId, HowManyUncompletedToDos());
            await ApiService.UpdateToDoListAsync(ToDoListModel);

            StateHasChanged();
        }

        private void EditListName()
        {
            _isEditing = true;
            _editListName = ToDoListModel.ListName;

            _shouldFocusInput = true;

            StateHasChanged();
        }

        private async void SaveEdit()
        {
            if (!string.IsNullOrWhiteSpace(_editListName))
            {
                TrimString(_editListName, _maxListNameLength);

                ToDoListModel.ListName = _editListName;
                OnListNameChanged(EventArgs.Empty);
                await OnUpdateListAsync();
            }

            _isEditing = false;
        }

        private async void DeleteList()
        {
            await ToDoService.DeleteListAsync(ListId);
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
