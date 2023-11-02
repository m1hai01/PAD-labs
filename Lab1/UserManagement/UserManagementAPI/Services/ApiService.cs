using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UserManagementAPI.Entities;
using UserManagementAPI.Interfaces;
using UserManagementAPI.Models;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly int _timeoutInSeconds = 20; // Set your desired timeout duration in seconds

    // Constructor for ApiService, initializing the HttpClient with base URL
    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5001"); // Base URL of File Management API
    }

    // Method to upload a file asynchronously with a timeout
    public async Task<string> UploadFileAsync(FileUploadRequest request)
    {
        // Create a CancellationTokenSource with the specified timeout duration
        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeoutInSeconds)))
        {
            // Serialize the request object to JSON and create a StringContent for the request body
            var jsonRequest = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                // Send a POST request to the File Management API with the specified timeout
                var response = await _httpClient.PostAsync("/api/files/upload", content, cts.Token);

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    // Read and deserialize the response content to ApiResponse object
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<ApiResponse>(responseContent);

                    // Check if the response contains 'fileId' and it's a success
                    if (responseObject != null && responseObject.Success)
                    {
                        return responseObject.FileId; // Return the fileId
                    }
                    else
                    {
                        // Handle the case where the response does not contain 'fileId'
                        return null;
                    }
                }
                else
                {
                    // Handle error response, throw exception, or return appropriate error message
                    return null;
                }
            }
            catch (TaskCanceledException)
            {
                // Handle timeout exception
                throw new TimeoutException("The request timed out.");
            }
            catch (HttpRequestException ex)
            {
                // Handle other HTTP request exceptions
                throw new Exception("An error occurred while making the request.", ex);
            }
        }
    }

    public void CreateUser(User user)
    {
        throw new NotImplementedException();
    }
}
