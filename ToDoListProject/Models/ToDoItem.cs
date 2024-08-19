using System.ComponentModel.DataAnnotations;

namespace ToDoListProject.Models
{
    public class ToDoItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Description is required")]
        public required string Description { get; set; }
        public bool Completed { get; set; }
    }
}