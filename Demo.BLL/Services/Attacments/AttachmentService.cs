using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Demo.BLL.Services.Attacments
{
    public class AttachmentService : IAttacchmentService
    {
        private readonly List<string> _allowedExtensions = new() { ".png", ".jpg", ".jpeg" };
        private const int _maxAllowedSize = 2_097_152; // 2MB

        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AttachmentService> _logger;

        public AttachmentService(IWebHostEnvironment env, ILogger<AttachmentService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string?> Upload(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("File nullo o vuoto.");
                return null;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Estensione non valida: {Extension}", extension);
                return null;
            }

            if (file.Length > _maxAllowedSize)
            {
                _logger.LogWarning("File troppo grande: {Size} bytes", file.Length);
                return null;
            }

            var folderPath = Path.Combine(_env.WebRootPath, "Files", folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                _logger.LogInformation("Cartella creata: {FolderPath}", folderPath);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                _logger.LogInformation("File salvato correttamente: {FilePath}", filePath);
                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel salvataggio del file: {FilePath}", filePath);
                return null;
            }
        }

        public bool Delete(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("File {FilePath} deleted successfully.", filePath);
                    return true;
                }
                else
                {
                    _logger.LogWarning("File {FilePath} not found for deletion.", filePath);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
                return false;
            }
        }
    }
}
