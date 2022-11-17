using ExtractorApi.Model;

namespace ExtractorApi.Helper
{
    public interface IExtractorHelper
    {
        Task<string> SaveData(CourseEvaluation CourseData);
        Task<IEnumerable<Evaluation>> MergeData(string term);

    }
}