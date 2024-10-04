using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using TodoList.Shared.DTOs;
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

        //protected override async Task OnParametersSetAsync()
        //{
        //    if (!string.IsNullOrEmpty(ListId))
        //    {
        //        ToDoListModel = await ApiService.GetToDoListAsync(ListId);

        //        if (ToDoListModel != null)
        //        {
        //            _uncompletedToDoItems = ToDoListModel.Items.Where(i => !i.Completed).ToList();
        //            _completedToDoItems = ToDoListModel.Items.Where(i => i.Completed).ToList();
        //        }

        //        await LoadTodoList();
        //    }
        //}

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(ListId))
            {
                await LoadTodoList();
            }
        }

        private async Task LoadTodoList()
        {
            _isLoading = true;

            try
            {
                // Get the ToDoListModel (if you still need this information)
                ToDoListModel = await ApiService.GetToDoListAsync(ListId);

                // Get the todos for this list using the new method
                var todos = await ToDoService.GetToDosForListAsync(ListId);

                // Update the ToDoListModel's Items if it exists
                if (ToDoListModel != null)
                {
                    ToDoListModel.Items = todos;
                }

                // Update the completed and uncompleted lists
                _uncompletedToDoItems = todos.Where(i => !i.Completed).ToList();
                _completedToDoItems = todos.Where(i => i.Completed).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading todo list: {ex.Message}");
                // You might want to set an error message property here to display to the user
            }
            finally
            {
                _isLoading = false;
            }
        }

        //private async Task LoadTodoList()
        //{
        //    _isLoading = true;

        //    try
        //    {
        //        ToDoListModel = await Http.GetFromJsonAsync<ToDoListModel>($"api/todolist/{ListId}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error loading todo list: {ex.Message}");
        //    }
        //    finally
        //    {
        //        _isLoading = false;
        //    }
        //}

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
            _preventDefault = e.Key == "Enter" && !e.ShiftKey;

            if (e.Key == "Enter" && !e.ShiftKey)
            {
                await AddNewItem();
            }
        }

        private async Task AddNewItem()
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
                var newItemDto = new CreateToDoItemDto
                {
                    Description = toDo,
                    Completed = false,
                    ToDoListModelId = ListId,
                };

                if (ToDoListModel != null)
                {
                    var addedToDo = await ToDoService.AddToDoAsync(newItemDto, ListId);
                    ToDoListModel.Items.Add(addedToDo);
                    _uncompletedToDoItems.Add(addedToDo);
                    _newToDoItem.Description = string.Empty;
                    await OnUpdateListAsync();
                }
            }
        }

        private async Task UpdateItemCompletionStatus(ToDoItem toDoItem)
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

            await ToDoService.UpdateToDoAsync(toDoItem);
            await OnUpdateListAsync();
        }

        private async Task HandleDeleteItem(ToDoItem toDoItem)
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

            await ToDoService.DeleteToDoAsync(toDoItem.Id);
            ToDoListModel.Items.Remove(toDoItem);

            await OnUpdateListAsync();
        }

        private async Task HandleReorder((int OldIndex, int NewIndex) reorderInfo)
        {
            var (oldIndex, newIndex) = reorderInfo;
            await ReorderToDos(oldIndex, newIndex);
        }

        public async Task ReorderToDos(int oldIndex, int newIndex)
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

        private async Task SaveEdit()
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

        private async Task DeleteList()
        {
            await ToDoService.DeleteListAsync(ListId);
            StateHasChanged();
        }

        private void ShowDeleteConfirmation()
        {
            _showDeleteConfirmation = true;
        }

        private async Task AcceptDeleteConfirmation(MouseEventArgs e)
        {
            _showDeleteConfirmation = false;
            await DeleteList();
        }

        private void HideDeleteConfirmation(MouseEventArgs e)
        {
            _showDeleteConfirmation = false;
        }

        #endregion
    }
}
