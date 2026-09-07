using AutoRecon.Models;
using AutoRecon.Services;
using AutoRecon.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AutoRecon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ReconServices _reconServices;
        private readonly AppDbContext _dbContext;

        public HomeController(ReconServices reconServices, AppDbContext dbContext)
        {
            _reconServices = reconServices;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id.HasValue)
            {
                var historicalScan = await _dbContext.Scans
                    .Include(s => s.Vulnerabilities)
                    .FirstOrDefaultAsync(s => s.ScanID == id.Value);

                if (historicalScan != null)
                {
                    var severityWeights = new Dictionary<string, int>
                    {
                        { "Critical", 1 },
                        { "High", 2 },
                        { "Medium", 3 },
                        { "Low", 4 }
                    };

                    historicalScan.Vulnerabilities = historicalScan.Vulnerabilities
                        .OrderBy(v => severityWeights.ContainsKey(v.Severity) ? severityWeights[v.Severity] : 5)
                        .ToList();

                    return View(historicalScan);
                }
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RunScan(string ipAddress)
        {
            // Check if the user exists, if not create a default user for local development
            var user = await _dbContext.Users.FirstOrDefaultAsync();
            if (user == null)
            {
                user = new User 
                { 
                    Username = "localdev",
                    Email = "admin@autorecon.local"
                };

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }
            // Check if the target exists, if not create a new target for the provided IP address
            var target = await _dbContext.Targets.FirstOrDefaultAsync();
            if (target == null)
            {
                target = new Target
                {
                    IPAddress = ipAddress,
                    Hostname = "Initial-Target", // Placeholder to avoid potential hostname constraints, can be updated later with actual hostname if needed
                    UserID = user.UserID // Use the actual user ID from the created or retrieved user
                };

                _dbContext.Targets.Add(target);
                await _dbContext.SaveChangesAsync();
            }

            var scanResult = await _reconServices.ScanAsync(target.TargetID, ipAddress);

            // Sort vulnerabilities by severity before passing to the view
            if (scanResult.Vulnerabilities != null && scanResult.Vulnerabilities.Any())
            {
                // Define severity weights for sorting
                var severityWeights = new Dictionary<string, int>
                {
                    { "Critical", 1 },
                    { "High", 2 },
                    { "Medium", 3 },
                    { "Low", 4 }
                };

                // Sort vulnerabilities by severity using the defined weights
                scanResult.Vulnerabilities = scanResult.Vulnerabilities
                    .OrderBy(v => severityWeights.ContainsKey(v.Severity) ? severityWeights[v.Severity] : 5)
                    .ToList();
            }

            return View("Index", scanResult);
        }

        // New action to display scan history
        public async Task<IActionResult> History()
        {
            var pastScans = await _dbContext.Scans
                .Include(s => s.Vulnerabilities)
                .OrderByDescending(s => s.Timestamp)
                .ToListAsync();

            return View(pastScans);
        }

        // New action to display settings page
        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
