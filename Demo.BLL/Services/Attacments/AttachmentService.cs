using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Services.Attacments
{
    public class AttachmentService :IAttacchmentService
    {
        public readonly List<string> _allowedExtentions = new() { ".png", ".jpg", ".jpeg" };
        //Max Size 2MB
        public const int _maxAllowedSize = 2_097_152;
        public string? Upload(IFormFile file, string folderName)
        {
            //1] Validate for extensions [".png", ".jpg", ".jpeg"]
            var extention = Path.GetExtension(file.FileName);
            if (!_allowedExtentions.Contains(extention))
                return null;

            //2] Validate for Max size[2_097_152; //2MB]
            if (file.Length > _maxAllowedSize)
                return null;

            //3] Get located folder path
            //   var folderPath = "E:\\dotnet\\Route\\C#\\MVC\\Session 03\\Assignment\\Group01MVC\\Demo.PL\\wwwroot\\Files\\Imags\\"
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", "Images");

            //4] Set unique file name
            //132321313.png
            var fileName = $"{Guid.NewGuid()}{extention}";

            //5] Get file path [FolderPath + FileName]
            var filePath = Path.Combine(folderPath, fileName);

            //6] Save file as stream [Data per time]
            using var fileStream = new FileStream(filePath, FileMode.Create);

            //7] Copy file to the stream
            file.CopyTo(fileStream);

            //8] Return file name
            return fileName;

        }

        public bool Delete(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
    }
}
