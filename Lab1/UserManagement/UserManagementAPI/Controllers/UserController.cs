using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using System.Text;
using Microsoft.Extensions.Logging;
using UserManagementAPI.Entities;
using UserManagementAPI.Interfaces;
using Microsoft.Extensions.Configuration;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IApiService _apiService;
        private readonly ILogger<UserController> _logger; // Add ILogger
        private string port = "";

        // Constructor for UserController, initializing ApiService and ILogger
        public UserController(IApiService apiService, ILogger<UserController> logger,IConfiguration configuration)
        {
            port = configuration.GetValue<string>("port")!;
            _apiService = apiService;
            _logger = logger; // Initialize the logger
        }

        // Endpoint for checking service status (GET /api/users/status)
        [HttpGet]
        [Route("status")]
        public IActionResult GetStatus()
        {
            try
            {
                // Log information and return a healthy status message
                _logger.LogInformation("Status endpoint accessed. Service is healthy.");
                return Ok(new { message = "User Management API is healthy" });
            }
            catch (Exception ex)
            {
                // Log error and return a 500 Internal Server Error response if an exception occurs
                _logger.LogError(ex, "Error occurred while checking service status.");
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        // Endpoint for registering a new user (POST /api/users/register)
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest request)
        {
            try
            {
                /*_apiService.CreateUser(new User
                {
                    FullName = request.Username,
                    Id = Guid.NewGuid()
                });*/

                // Log user registration success and return a success message
                _logger.LogInformation("User registered successfully.");
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                // Log registration error and return a 500 Internal Server Error response if an exception occurs
                _logger.LogError(ex, "Error occurred during user registration.");
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        // Endpoint for uploading a file (POST /api/users/upload)
        [HttpPost]
        [Route("upload")]
        /*public IActionResult UploadFile()
        {
            try
            {
                // Simulate an HttpRequestException for testing purposes
                throw new HttpRequestException("Simulated HttpRequestException in the upload endpoint.");

                // The actual implementation logic would typically follow, but it won't be executed in this example due to the exception.
            }
            catch (HttpRequestException ex)
            {
                // Optionally log the exception details for debugging or analysis
                Console.WriteLine($"HttpRequestException caught: {ex.Message}");

                // Rethrow the exception to ensure it is recognized by Polly's circuit breaker
                throw;
            }
        }*/

        public async Task<IActionResult> UploadFile()
        {
            try
            {
                // Read the file and create a FileUploadRequest object
                var path = Directory.GetCurrentDirectory();
                var filePath = $@"{path}\1.txt";
                byte[] fileBytes;

                try
                {
                    fileBytes = System.IO.File.ReadAllBytes(filePath);
                }
                catch (Exception ex)
                {
                    // Handle file read error, log the error, and return a 500 Internal Server Error response
                    _logger.LogError(ex, $"Error reading the file: {ex.Message}");
                    return StatusCode(500, new { message = "Internal Server Error" });
                }

                // Create a file upload request and upload the file using ApiService
                var fileUploadRequest = new FileUploadRequest
                {
                    FileName = Path.GetFileName(filePath),
                    FileData = fileBytes
                };

                var fileId = await _apiService.UploadFileAsync(fileUploadRequest);

                // Log successful file upload and return success message with file ID
                _logger.LogInformation("File uploaded successfully. File ID: {fileId}", fileId);
                return Ok(new { message = "File uploaded successfully", fileId });
            }
            catch (Exception ex)
            {
                // Log file upload error and return a 500 Internal Server Error response if an exception occurs
                _logger.LogError(ex, "Error occurred during file upload.");
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        [HttpGet("test_breaker")]
        public IActionResult TestBreaker()
        {
            _logger.LogInformation($"{port} port");
            if (port == "5000")
                return Ok("good breaker");

            return StatusCode(503, "testing internal server error");
        }

    }
}
