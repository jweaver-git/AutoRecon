using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text;

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

        public string TargetIP
        {
            get
            {
                if (string.IsNullOrEmpty(RawJSON)) return "Unknown Target";
                try
                {
                    var doc = JsonDocument.Parse(RawJSON);
                    return doc.RootElement.GetProperty("target").GetString();
                }
                catch
                {
                    return "N/A";
                }
            }
        }
        
        public string? TrueRawNmapOutput { get; set; }

        public string FormattedNmapOutput
        {
            get
            {
                if (string.IsNullOrEmpty(RawJSON)) return "Waiting for scan initialization...";

                try
                {
                    // Parse the raw JSON and format it into a human-readable string
                    var doc = JsonDocument.Parse(RawJSON);
                    var root = doc.RootElement;

                    var targetIp = root.GetProperty("target").GetString();
                    var state = root.GetProperty("state").GetString();

                    var sb = new StringBuilder();
                    sb.AppendLine($"[+] Starting AutoRecon Nmap Scan...");
                    sb.AppendLine($"[+] Discovering open ports on {targetIp} (State: {state})");

                    if (root.TryGetProperty("open_ports", out var openPorts))
                    {
                        foreach (var port in openPorts.EnumerateArray())
                        {
                            var portNum = port.GetProperty("port").GetInt32();
                            var service = port.GetProperty("service").GetString();

                            sb.AppendLine($"[+] Port {portNum} open ({service})");
                        }
                    }
                    sb.AppendLine($"[+] Parsing to JSON payload... Done.");

                    return sb.ToString();
                }
                catch
                {
                    // If parsing fails, return the raw JSON for app stability
                    return RawJSON;
                }
            }
        }
    }
}
