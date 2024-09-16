using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Concurrent;
using ToDoList.Shared.Models;

namespace ToDoListProject.Services
{
    public class ToDoService
    {
        public event EventHandler ListUpdated;

        private readonly NavigationService _navigationService;
        private readonly ApiService _apiService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private List<ToDoListModel> _toDoLists = new List<ToDoListModel>();
        private ConcurrentDictionary<string, int> _uncompletedCounts = new ConcurrentDictionary<string, int>();

        public ToDoService(NavigationService navigationService, ApiService apiService, AuthenticationStateProvider authenticationStateProvider)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<List<ToDoListModel>> GetAllListsAsync()
        {
            _toDoLists = await _apiService.GetAllToDoListsAsync();
            return _toDoLists;
        }

        public async Task<ToDoListModel> AddListAsync(ToDoListModel newList)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                var userIdClaim = user.FindFirst("UserId");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    newList.UserId = userId;
                    var addedList = await _apiService.AddToDoListAsync(newList);

                    if (addedList == null)
                    {
                        throw new Exception("Failed to add the new list. The response from the server was null.");
                    }

                    return addedList;
                }
                else
                {
                    Console.WriteLine("Invalid or missing UserId claim");
                    throw new UnauthorizedAccessException("UserId claim is missing or invalid.");
                }
            }
            else
            {
                Console.WriteLine("User is not authenticated");
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
        }

        public async Task<bool> UpdateListAsync(ToDoListModel updatedList)
        {
            try
            {
                await _apiService.UpdateToDoListAsync(updatedList);

                var index = _toDoLists.FindIndex(l => l.Id == updatedList.Id);

                if (index != -1)
                {
                    _toDoLists[index] = updatedList;
                    OnListUpdated(EventArgs.Empty);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating list: {ex.Message}");
                return false;
            }
        }

        public async Task DeleteListAsync(string listId)
        {
            await _apiService.DeleteToDoListAsync(listId);

            var listToDelete = _toDoLists.FirstOrDefault(l => l.Id == listId);

            if(listToDelete != null)
            {
                _toDoLists.Remove(listToDelete);
                OnListUpdated(EventArgs.Empty);

                if(_toDoLists.Any())
                {
                    _navigationService.NavigateToList($"{_toDoLists.First().Id}");
                }
                else
                {
                    _navigationService.NavigateTo("/");
                }
            }
        }

        protected virtual void OnListUpdated(EventArgs e)
        {
            ListUpdated?.Invoke(this, e);
        }

        public void SetUncompletedCount(string listId, int count)
        {
            _uncompletedCounts[listId] = count;
            OnListUpdated(EventArgs.Empty);
        }

        public int GetUncompletedCount(string listId)
        {
            return _uncompletedCounts.TryGetValue(listId, out int count) ? count : 0;
        }
    }
}
