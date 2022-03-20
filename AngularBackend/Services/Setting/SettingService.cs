using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;
using Shared.Settings;

namespace AngularBackend.Services.Setting
{
    public class SettingService : ISettingService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public SettingService(IConfiguration configuration, ILogger<SettingService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public T GetSettings<T>()
        {
            if (typeof(T) == typeof(FileUploadSettings))
            {
                FileUploadSettings fileUploadSettings = new FileUploadSettings()
                {
                    UploadMaxAllowedFiles = int.Parse(_configuration["FileUploadSettings:UploadMaxAllowedFiles"]),
                    MaxFileSize = long.Parse(_configuration["FileUploadSettings:MaxFileSize"])
                };

                // object型にキャストしてからTにキャストします
                // 直接Tにキャストしようとするとコンパイルエラーとなります
                return (T)(object)fileUploadSettings;
            }

            throw new ArgumentOutOfRangeException($"{nameof(T)}が見つかりませんでした");
        }

    }
}
