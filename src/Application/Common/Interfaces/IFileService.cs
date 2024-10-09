using Microsoft.AspNetCore.Http;

namespace warehouse_BE.Application.Common.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions);
    void DeleteFile(string fileNameWithExtension);
}
