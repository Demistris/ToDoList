using System.ComponentModel.DataAnnotations;

namespace ToDoListProject.Models
{
    public class ToDoListModel
    {
        [Key]
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string ListName { get; set; } = string.Empty;
        public List<ToDoItem> Items { get; set; } = new List<ToDoItem>();
        public bool IsEditing = false;
    }
}