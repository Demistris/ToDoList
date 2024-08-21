using System.ComponentModel.DataAnnotations;

namespace ToDoListProject.Models
{
    public class ToDoListModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ListName { get; set; } = string.Empty;
        public List<ToDoItem> Items { get; set; } = new List<ToDoItem>();
    }
}