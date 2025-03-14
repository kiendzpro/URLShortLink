using System.ComponentModel.DataAnnotations;

namespace WebApplication10.Models
{
    public class UrlShortenRequest
    {
        [Required(ErrorMessage = "Please enter a URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [Display(Name = "URL to Shorten")]
        public string Url { get; set; }
    }
} 