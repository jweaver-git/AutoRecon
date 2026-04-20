using System.ComponentModel.DataAnnotations;

namespace AutoRecon.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        // Navigation properties
        public ICollection<Target> Targets { get; set; }
    }
}
