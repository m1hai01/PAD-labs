using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserManagementAPI.Interfaces;
using UserManagementAPI.Models;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5001"); // Base URL of File Management API
    }

    public async Task<string> UploadFileAsync(FileUploadRequest request)
    {
        var jsonRequest = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/files/upload", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ApiResponse>(responseContent);

            if (responseObject != null && responseObject.Success)
            {
                return responseObject.FileId;
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
}