using System.Net;
using AngularBackend.Services;
using AngularBackend.Services.FileUpload;
using AngularBackend.Services.Setting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Data;
using Shared.Settings;


namespace AngularBackend.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        
        private readonly ILogger<UploadController> _logger;
        private readonly IFileUploadService _uploadService;
        private readonly ISettingService _settingService;
        

        public UploadController(IFileUploadService uploadService, ISettingService settingService)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddJsonConsole(options => options.IncludeScopes = true);
            });
            _logger = loggerFactory.CreateLogger<UploadController>();

            _uploadService = uploadService;
            _settingService = settingService;
        }

        [HttpGet("csv")]
        public ActionResult Get()
        {
            return Ok();
        }

        [HttpPut("csv")]
        public void Put()
        {
            _logger.LogDebug("Put");
        }

        [HttpPost]
        public void Post()
        {
            var body = Request.Body;
            var file = Request.Form.Files;
            _logger.LogDebug("Post");
        }

        /// <summary>
        /// ASP.NET Core Blazor ファイルのアップロード
        /// https://docs.microsoft.com/ja-jp/aspnet/core/blazor/file-uploads?view=aspnetcore-6.0&pivots=server
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("address")]
        public async Task<ActionResult<IList<UploadResult>>> PostFile()
        {
            var body = Request.Body;
            var file = (FormFile)Request.Form.Files[0];
            IFormFile[] formFiles = new IFormFile[1]{ file };

            _logger.LogInformation($"ファイルを受信 PostFile: {formFiles.Count()}件");

            var uploadResults = await _uploadService.UploadAsync(formFiles);
            var paths = _uploadService.PathStrings;
            await _uploadService.ReadCsvAndSaveToDb(paths);

            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}");

            return new CreatedResult(resourcePath, uploadResults);

        }
        // Request body => [FromBody]
        // Form data in the request body => [FromForm]
        // Headers => [FromHeader]
        // Query string parameter => [FromQuery]

        [HttpGet("settings")]
        public ActionResult<IList<string>> SettingsController()
        {
            var result = _settingService.GetSettings<FileUploadSettings>();
            List<string> list = new List<string>();
            list.Add($"MaxFileSize is {result.MaxFileSize}");
            list.Add($"UploadMaxAllowedFiles is {result.UploadMaxAllowedFiles}");

            return list;
        }

    }
}
