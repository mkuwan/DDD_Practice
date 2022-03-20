using System.Net;
using AngularBackend.Services.Csv;
using AngularBackend.Services.Setting;
using Domain.Repository;
using Shared.Converters;
using Shared.Data;
using Shared.Settings;

namespace AngularBackend.Services.FileUpload
{
    public class FileUploadService : IFileUploadService
    {
        private readonly ILogger<FileUploadService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ISettingService _settingService;
        private readonly IImportAddressRepository _importAddressRepository;

        public string[] PathStrings { get; private set; }

        public FileUploadService(
            ILogger<FileUploadService> logger, 
            IWebHostEnvironment webHostEnvironment, 
            IConfiguration configuration, 
            ISettingService settingService, IImportAddressRepository importAddressRepository)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _settingService = settingService;
            _importAddressRepository = importAddressRepository;
        }


        /// <summary>
        /// ファイルアップロード
        /// </summary>
        /// <param name="files">web api で受信したファイル</param>
        /// <returns></returns>
        public async Task<List<UploadResult>> UploadAsync(IEnumerable<IFormFile> files)
        {
            var result = _settingService.GetSettings<FileUploadSettings>();

            var maxAllowedFiles = result.UploadMaxAllowedFiles;
            long maxFileSize = result.MaxFileSize;

            var filesProcessed = 0;

            List<UploadResult> uploadResults = new();
            PathStrings = new string[files.Count()];
            int count = 0;

            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                var untrustedFileName = file.FileName;
                var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

                uploadResult.FileName = untrustedFileName;

                if (filesProcessed < maxAllowedFiles)
                {
                    if (file.Length == 0)
                    {
                        _logger.LogInformation("{FileName} length is 0 (Err: 1)", trustedFileNameForDisplay);
                        uploadResult.ErrorCode = 1;
                    }
                    else if (file.Length > maxFileSize)
                    {
                        _logger.LogInformation("{FileName} of {Length} bytes is larger than th limit of {Limit} bytes (Err: 2)",
                            trustedFileNameForDisplay, file.Length, maxFileSize);
                        uploadResult.ErrorCode = 2;
                    }
                    else
                    {
                        try
                        {
                            var trustedFileNameForFileStorage = Path.GetRandomFileName();
                            var path = Path.Combine(_webHostEnvironment.ContentRootPath, _webHostEnvironment.EnvironmentName,
                                "unsafe_uploads", trustedFileNameForFileStorage);
                            PathStrings[count] = path;
                            count++;

                            // file copy
                            await using FileStream fs = new(path, FileMode.Create);
                            await file.OpenReadStream().CopyToAsync(fs);

                            uploadResult.Uploaded = true;
                            uploadResult.StoredFileName = trustedFileNameForFileStorage;
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                                trustedFileNameForDisplay, exception.Message);
                            uploadResult.ErrorCode = 3;
                        }
                    }

                    filesProcessed++;
                }
                else
                {
                    _logger.LogInformation("{FileName} not uploaded because the " +
                                          "request exceeded the allowed {Count} of files (Err: 4)",
                        trustedFileNameForDisplay, maxAllowedFiles);
                    uploadResult.ErrorCode = 4;
                }
                uploadResults.Add(uploadResult);
            }

            return uploadResults;
        }

        /// <summary>
        /// DBへの保存処理
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public async Task ReadCsvAndSaveToDb(string[] paths)
        {
            var path = ExtensionConverter.ChangeExtension(paths[0], "csv");
            ImportAddressCsv import = new ImportAddressCsv(_importAddressRepository);
            var data = await import.ReadCsvAsync(path);
            await import.SaveToDb(data);
        }
    }
}
