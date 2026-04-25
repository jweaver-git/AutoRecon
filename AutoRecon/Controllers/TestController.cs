using Microsoft.AspNetCore.Mvc;
using AutoRecon.Services;
using AutoRecon.Data;
using AutoRecon.Models;

namespace AutoRecon.Controllers
{
    public class TestController : Controller
    {
        private readonly ReconServices _reconServices;
        private readonly AppDbContext _dbContext;

        public TestController(ReconServices reconServices, AppDbContext dbContext)
        {
            _reconServices = reconServices;
            _dbContext = dbContext;
        }

        [HttpGet("/run-test")]
        public async Task<IActionResult> RunTest()
        {
            try
            {
                var target = _dbContext.Targets.FirstOrDefault();
                if (target == null)
                {
                    var user = new User { Username = "TestAdmin", Email = "admin@autorecon.local" };
                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync();

                    target = new Target { UserID = user.UserID, IPAddress = "127.0.0.1", Hostname = "Localhost" };
                    _dbContext.Targets.Add(target);
                    await _dbContext.SaveChangesAsync();
                }

                var newScan = await _reconServices.ScanAsync(target.TargetID, target.IPAddress);

                return Json(newScan);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error running test: {ex.Message}");
            }
        }
    }
}
