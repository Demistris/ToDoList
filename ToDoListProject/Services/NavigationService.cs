using Microsoft.AspNetCore.Components;

namespace ToDoListProject.Services
{
    public class NavigationService
    {
        private readonly NavigationManager _navigationManager;

        public NavigationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public void NavigateToList(string listId)
        {
            _navigationManager.NavigateTo($"/list/{listId}");
        }

        public void NavigateToHomePage()
        {
            _navigationManager.NavigateTo("/");
        }
    }
}
