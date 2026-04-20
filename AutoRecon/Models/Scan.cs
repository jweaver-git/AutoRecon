using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRecon.Models
{
    public class Scan
    {
        [Key]
        public int ScanID { get; set; }

        [ForeignKey("Target")]
        public int TargetID { get; set; }
        public Target Target { get; set; }

        public DateTime Timestamp { get; set; }
        public string RawJSON { get; set; }

        // Navigation properties
        public ICollection<Vulnerability> Vulnerabilities { get; set; }
    }
}
