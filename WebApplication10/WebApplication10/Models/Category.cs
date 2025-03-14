using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication10.Models
{
    public class Category
    {
        public Category()
        {
            Todos = new List<Todo>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<Todo> Todos { get; set; }
    }
}
