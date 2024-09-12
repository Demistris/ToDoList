using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Models
{
    public class ToDoItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Description is required")]
        public required string Description { get; set; }
        public bool Completed { get; set; }
        public string ToDoListModelId { get; set; }
        public ToDoListModel ToDoListModel { get; set; }
    }
}