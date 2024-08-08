using System.ComponentModel.DataAnnotations;

namespace ToDoListProject.Models
{
    public class ToDoItem
    {
        [Required(ErrorMessage = "Description is required")]
        public required string Description { get; set; }
        public bool Completed { get; set; }
        //Order?
    }
}