using System.ComponentModel.DataAnnotations;

namespace demo_docker.Dto
{
    public class AddNotes
    {
        [Required]
        public string Message { get; set; }

        public string? Passoword { get; set; }

        [Required]
        public bool IsPassoword { get; set; }
    }
}
