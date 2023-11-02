﻿using UserManagementAPI.Entities;
using UserManagementAPI.Models;

namespace UserManagementAPI.Interfaces
{
    public interface IApiService
    {
        Task<string> UploadFileAsync(FileUploadRequest request);
        void CreateUser(User user);
    }
}
