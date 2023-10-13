﻿namespace TestBlobStorage.Services
{
    public interface IStorageManager
    {
        string GetSignedUrl(string fileName);
        Task<string> GetSadeUrlAsync(string fileName);
        Task<string> GetSignedUrlAsync(string fileName);
        bool UploadFile(Stream stream, string fileName, string contentType);
        Task<bool> UploadFileAsync(Stream stream, string fileName, string contentType);

        bool DeleteFile(string fileName);
        Task<bool> DeleteFileAsync(string fileName);
    }
}
