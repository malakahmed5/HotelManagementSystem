using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Abstraction
{
    public interface IAttachmentService
    {
        Task<string?> UploadFile(IFormFile file, string folderName);
        bool DeleteFile(string folderName , string fileName);
    }
}
