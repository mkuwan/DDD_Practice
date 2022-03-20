using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Settings
{
    public class FileUploadSettings
    {
        /// <summary>
        /// 同時にアップロード可能なファイル数
        /// </summary>
        public int UploadMaxAllowedFiles { get; set; }

        /// <summary>
        /// ファイル最大サイズ
        /// Default: 10485760(1024 * 1024 * 10 = 10MB)
        /// </summary>
        public long MaxFileSize { get; set; }
    }
}
