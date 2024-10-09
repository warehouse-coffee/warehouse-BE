using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Infrastructure.Service;


public class FileService : IFileService
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    public FileService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        this._configuration = configuration;
        this._environment = webHostEnvironment;
    }

    public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions)
    {
        if (imageFile == null)
        {
            throw new ArgumentNullException(nameof(imageFile));
        }

        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath, "wwwroot", "images", "avatars");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // Check the allowed extenstions
        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
        {
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
        }

        // generate a unique filename
        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        try
        {
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
        }
        catch (Exception ex)
        {
            // Ghi log lỗi hoặc xử lý lỗi
            throw new Exception($"Error saving file: {ex.Message}");
        }
        return fileName;
    }


    public void DeleteFile(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
        {
            throw new ArgumentNullException(nameof(fileNameWithExtension));
        }
        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath, $"wwwroot", "images", "avatars", fileNameWithExtension);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Invalid file path");
        }
        File.Delete(path);
    }

}
