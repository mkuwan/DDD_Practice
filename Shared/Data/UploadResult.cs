namespace Shared.Data
{
    /// <summary>
    /// Upload result class
    /// https://docs.microsoft.com/ja-jp/aspnet/core/blazor/file-uploads?view=aspnetcore-6.0&pivots=server
    /// </summary>
    public class UploadResult
    {
        public bool Uploaded { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
        public int ErrorCode { get; set; }
    }
}
