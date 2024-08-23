using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MottuMotoRental.Core.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string fileName);
    }
}