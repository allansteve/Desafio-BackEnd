using MottuMotoRental.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace MottuMotoRental.Infrastructure.FileStorage
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _storagePath;

        public LocalFileStorageService()
        {
            _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "TempImages");


            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName)
        {
            var filePath = Path.Combine(_storagePath, fileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filePath;
        }
    }
}