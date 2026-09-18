using HMS.Services.Abstraction;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Services.Helpers
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AttachmentService> _logger;
        private readonly long _maxFileSize = 5* 1024 * 1024;
        private readonly string[] _allowedExtensions = {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg",
            ".mp4", ".mov", ".avi", ".mkv", ".webm", ".flv", ".wmv" };

        public AttachmentService(IWebHostEnvironment environment , ILogger<AttachmentService> logger)
        {
            _environment = environment;
            _logger = logger;
        }
        public async Task<string?> UploadFile(IFormFile file, string folderName)
        {
            try
            {
                if (file is null || string.IsNullOrWhiteSpace(folderName))
                    return null;

                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!_allowedExtensions.Contains(extension) || file.Length > _maxFileSize)
                    return null;

                var folderPath = Path.Combine(_environment.WebRootPath,"images", folderName);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(folderPath, fileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);

                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An UnExcpected Error Happened While Uploading Image On Stream");
                return null;
            }
        }

        public bool DeleteFile(string folderName, string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(fileName))
                    return false;

                var filePath = Path.Combine(_environment.WebRootPath,"images" ,folderName, fileName);

                if (!File.Exists(filePath))
                    return false;

                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected Error Happened While Delete Image From Server");
                return false;
            }
        }
    }
}
