using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Models
{
    public class ToDoListModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ListName { get; set; } = string.Empty;
        public List<ToDoItem> Items { get; set; } = new List<ToDoItem>();

        public int UserId { get; set; }
    }
}