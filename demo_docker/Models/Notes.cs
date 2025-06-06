using System.ComponentModel.DataAnnotations;

namespace demo_docker.Models
{
    public class Notes
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Message { get; set; }

        public string? Passoword { get; set; }

        [Required]
        public bool IsPassoword { get; set; }
    }
}
