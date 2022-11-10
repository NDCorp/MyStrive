using ExtractorApi.Model;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ExtractorApi.Helper
{
    public class ExtractorHelper
    {
        private const string DIRECTORY = @"\\jokas1\MyStriveExtracted\";
        private const string FILE_EXTENSION = ".json";
        private Regex RegexToReplace => new Regex(@"\s+");

        public async Task<string> SaveData(CourseEvaluation CourseData)
        {
            // Create json file
            var fileName = CreateFileName(CourseData.Name, CourseData.Term);
            var path = Path.Combine(DIRECTORY, $"{fileName}{FILE_EXTENSION}");

            await using (StreamWriter writer = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, CourseData);
            }

            return CourseData.Term;
        }

        public async Task<IEnumerable<Evaluation>> MergeData(string term)
        {
            // Create json file from all syllabus data for the term
            var path = Path.Combine(DIRECTORY, $"{nameof(term)}{term}{FILE_EXTENSION}");

            var courses = new List<CourseEvaluation>();

            foreach (var f in Directory.GetFiles(DIRECTORY, $"{term}*.json", SearchOption.TopDirectoryOnly))
            {
                using (StreamReader reader = File.OpenText(f))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var courseData = (CourseEvaluation)serializer.Deserialize(reader, typeof(CourseEvaluation));
                    courses.Add(courseData);
                }
            }

            // Create the file with all syllabus data for the term
            var mergedData = courses.SelectMany(x => x.Evaluations).OrderBy(x => x.DueDate).ThenByDescending(x => x.ActualWeightPerc);
            await using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, mergedData);
            }

            //return await Task.FromResult(data);
            return mergedData;
        }

        private string CreateFileName(string course, string session)
        {
            string course1 = RegexToReplace.Replace(course, "");
            string session1 = RegexToReplace.Replace(session, "");

            return string.Format($"{session1}_{course1}");
        }
    }
}
