using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models; 
using System.Text;
using Microsoft.Extensions.Logging;
using UserManagementAPI.Interfaces; 

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IApiService _apiService;
        private readonly ILogger<UserController> _logger; // Add ILogger

        public UserController(IApiService apiService, ILogger<UserController> logger)
        {
            _apiService = apiService;
            _logger = logger; // Initialize the logger
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest request)
        {
            try
            {

                _logger.LogInformation("User registered successfully.");

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration.");
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                // Read the file and create a FileUploadRequest object
                var filePath = @"D:\1.txt"; 
                byte[] fileBytes;

                try
                {
                    fileBytes = System.IO.File.ReadAllBytes(filePath);
                }
                catch (Exception ex)
                {
                    // Handle file read error
                    _logger.LogError(ex, $"Error reading the file: {ex.Message}");
                    return StatusCode(500, new { message = "Internal Server Error" });
                }

                var fileUploadRequest = new FileUploadRequest
                {
                    FileName = Path.GetFileName(filePath),
                    FileData = fileBytes
                };

                var fileId = await _apiService.UploadFileAsync(fileUploadRequest);

                _logger.LogInformation("File uploaded successfully. File ID: {fileId}", fileId);
                return Ok(new { message = "File uploaded successfully", fileId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during file upload.");
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

    }
}
