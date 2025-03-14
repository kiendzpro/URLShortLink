using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication10.Models
{
    public enum TodoStatus
    {
        NotStarted,
        InProgress,
        Completed
    }

    public class Todo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public TodoStatus Status { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
