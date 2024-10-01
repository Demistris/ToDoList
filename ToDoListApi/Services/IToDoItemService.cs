﻿using ToDoList.Shared.Models;

namespace ToDoListApi.Services
{
    public interface IToDoItemService
    {
        Task<ToDoItem> AddToDoAsync(ToDoItem newToDo);
        Task UpdateToDoAsync(ToDoItem updatedToDo);
        Task DeleteToDoAsync(string toDoId);
        Task<ToDoItem> GetToDoByIdAsync(string toDoId);
    }
}