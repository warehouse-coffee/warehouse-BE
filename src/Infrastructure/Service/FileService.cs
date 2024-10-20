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

        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
        {
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
        }

        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        try
        {
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
        }
        catch (Exception ex)
        {
            
            throw new Exception($"Error saving file: {ex.Message}");
        }
        return fileName;
    }


    public void DeleteFile(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
        {
            Console.WriteLine($"File name cannot be null or empty: {nameof(fileNameWithExtension)}");
            return; 
        }
        var contentPath = _environment.ContentRootPath;
        var path = Path.Combine(contentPath, $"wwwroot", "images", "avatars", fileNameWithExtension);

        if (!File.Exists(path))
        {
            Console.WriteLine($"File not found at path: {path}. Unable to delete.");
            return; 
        }
        File.Delete(path);
    }

}
