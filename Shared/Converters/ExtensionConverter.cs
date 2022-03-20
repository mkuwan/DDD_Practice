using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Converters
{
    public static class ExtensionConverter
    {
        public static string ChangeExtension(string filePath, string extension)
        {
            if (File.Exists(filePath))
            {
                var newFilePath = Path.ChangeExtension(filePath, extension);
                File.Move(filePath, newFilePath);
                return newFilePath;
            }

            return String.Empty;
        }
    }
}
