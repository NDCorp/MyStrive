using ExtractorApi.Model;

namespace ExtractorApi.Helper
{
    public interface IAzureStorageHelper
    {
        Task CreateShareAndFileAsync(string fileName, byte[] byteObject, string shareName, string directoryName);
        Task<List<CourseEvaluation>> ReadShareFilesAsync(string fileName, string shareName, string directoryName);
    }
}