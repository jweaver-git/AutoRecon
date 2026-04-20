using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRecon.Models
{
    public class Target
    {
        [Key]
        public int TargetID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        public string IPAddress { get; set; }
        public string Hostname { get; set; }

        // Navigation properties
        public ICollection<Scan> Scans { get; set; }
    }
}
