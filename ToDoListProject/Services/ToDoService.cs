using Blazorise;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using ToDoListProject.Models;

namespace ToDoListProject.Services
{
    public class ToDoService
    {
        public List<ToDoListModel> GetAllLists() => _toDoLists;

        private List<ToDoListModel> _toDoLists = new List<ToDoListModel>();
        private string _newListName = "Untitled";
        private readonly NavigationService _navigationService;

        public ToDoService(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public async Task<ToDoListModel> AddList()
        {
            var newList = new ToDoListModel
            {
                ListName = _newListName,
                Items = new List<ToDoItem>()
            };

            _toDoLists.Add(newList);
            await Task.CompletedTask;

            return newList;
        }

        public event EventHandler ListDeleted;

        protected virtual void OnListDeleted(EventArgs e)
        {
            ListDeleted?.Invoke(this, e);
        }

        public void DeleteList(string listId)
        {
            var listToDelete = _toDoLists.FirstOrDefault(l => l.Id == listId);

            if (listToDelete != null)
            {
                _toDoLists.Remove(listToDelete);
                OnListDeleted(EventArgs.Empty);

                if (_toDoLists.Any())
                {
                    _navigationService.NavigateToList($"{_toDoLists.First().Id}");
                }
                else
                {
                    _navigationService.NavigateToHomePage();
                }
            }
        }

        public async Task UpdateList(ToDoListModel updatedList)
        {
            var index = _toDoLists.FindIndex(l => l.Id == updatedList.Id);

            if (index != -1)
            {
                _toDoLists[index] = updatedList;
            }

            await Task.CompletedTask;
        }

        public ToDoListModel GetList(string listId) => _toDoLists.FirstOrDefault(l => l.Id == listId);
    }
}
