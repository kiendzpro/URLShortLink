using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication10.Models
{
    public class ShortenedUrl
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Url]
        [Display(Name = "Original URL")]
        public string OriginalUrl { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Short Code")]
        public string ShortCode { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Access Count")]
        public int AccessCount { get; set; } = 0;
    }
} 