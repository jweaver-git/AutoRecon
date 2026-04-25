using System.Text;
using System.Text.Json;
using AutoRecon.Data;
using AutoRecon.Models;

namespace AutoRecon.Services
{
    public class ReconServices
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;

        public ReconServices(HttpClient httpClient, AppDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<Scan> ScanAsync(int targetId, string ipAddress)
        {
            // Prepare the payload for the API request
            var payload = new { ip_address = ipAddress };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Send the POST request to the API
            var response = await _httpClient.PostAsync("http://localhost:8000/api/scan", content);

            // Ensure the response is successful. Throws error if not.
            response.EnsureSuccessStatusCode();

            // Read the raw JSON response from the API
            var rawJsonResponse = await response.Content.ReadAsStringAsync();

            // Save the exact JSON response to the database
            var newScan = new Scan
            {
                TargetID = targetId,
                Timestamp = DateTime.UtcNow,
                RawJSON = rawJsonResponse
            };

            _dbContext.Scans.Add(newScan);
            await _dbContext.SaveChangesAsync();

            return newScan;
        }
    }
}
