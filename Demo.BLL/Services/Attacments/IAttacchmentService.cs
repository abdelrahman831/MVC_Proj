using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Services.Attacments
{
    public interface IAttacchmentService 

    {

        public string? Upload(IFormFile file, string folderName);
        public bool Delete(string filePath);
    }
}
