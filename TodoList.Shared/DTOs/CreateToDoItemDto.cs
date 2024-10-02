using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Shared.DTOs
{
    public class CreateToDoItemDto
    {
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public bool Completed { get; set; }

        [Required]
        public string ToDoListModelId { get; set; }
    }
}
