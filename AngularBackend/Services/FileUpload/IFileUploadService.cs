using Shared.Data;

namespace AngularBackend.Services.FileUpload
{
    public interface IFileUploadService
    {
        Task<List<UploadResult>> UploadAsync(IEnumerable<IFormFile> files);
        string[] PathStrings { get; }

        Task ReadCsvAndSaveToDb(string[] paths);
    }
}
