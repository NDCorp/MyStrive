using ExtractorApi.Model;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Xml.Linq;

namespace ExtractorApi.Helper
{
    public class ExtractorHelper : IExtractorHelper
    {
        private const string STORAGE1 = "Azure:Configuration:AppSettings:Storage1:{0}:value";
        private const string SHARE_SETTING_NBR = "3";
        private const string DIRECTORY_SETTING_NBR = "4";

        private string ShareSyllabuses { get; set; }        //@"\\jokas1\MyStriveExtracted";
        private string DirectorySyllabuses { get; set; }    //@"syllabuses";
        private Regex RegexFindEmptyChar => new Regex(@"\s+");

        private readonly ILogger<AzureStorageHelper> _logger;
        private readonly IAzureStorageHelper _azureStorageHelper;
        private readonly IConfiguration _configuration;

        public const string FILE_EXTENSION = ".json";

        public ExtractorHelper(ILogger<AzureStorageHelper> logger, IAzureStorageHelper azureStorageHelper, IConfiguration configuration)
        {
            _logger = logger;
            _azureStorageHelper = azureStorageHelper;
            _configuration = configuration;

            ShareSyllabuses = _configuration.GetValue<string>(string.Format(STORAGE1, SHARE_SETTING_NBR));
            DirectorySyllabuses = _configuration.GetValue<string>(string.Format(STORAGE1, DIRECTORY_SETTING_NBR));
    }

        public async Task<string> SaveData(CourseEvaluation courseData)
        {
            // Create json file
            var fileName = FormatFileName(courseData.Name, courseData.Term);
            fileName += FILE_EXTENSION;

            var path = Path.Combine(@"\\jokas1", ShareSyllabuses, DirectorySyllabuses);
            await WriteFile(path, fileName, courseData);
            return courseData.Term;
        }

        public async Task<IEnumerable<Evaluation>> MergeData(string term)
        {
            var courses = new List<CourseEvaluation>();

            // Create json file from all syllabus data for the term
            var fileName = $"{nameof(term)}{term}{FILE_EXTENSION}";
            var path = Path.Combine(@"\\jokas1", ShareSyllabuses, DirectorySyllabuses);

            courses = await ReadFile(path, term);

            // Create the file with all syllabus data for the term
            //var mergedData = courses.SelectMany(x => x.Evaluations).OrderBy(x => x.DueDate).ThenByDescending(x => x.ActualWeightPerc);
            var mergedData = courses.SelectMany(x => x.Evaluations).OrderByDescending(x => x.ActualWeightPerc);

            await WriteFile(path, fileName, mergedData);
            return mergedData;
        }

        private async Task WriteFile<T>(string path, string fileName, T courseData)
        {
            if (Debugger.IsAttached)
            {
                // Create file locally if debugging:
                // You must have a local share accessible from (see appsettings files): \\ComputerName\MyStriveExtracted\syllabuses
                path = Path.Combine(path, fileName);
                await using (StreamWriter stream = File.CreateText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(stream, courseData);
                }
            }
            else
            {
                // Convert object to json, Get corresponding bytes, and Create file in Azure; if Release mode
                var jsonObject = JsonConvert.SerializeObject(courseData, Formatting.Indented);
                var byteObject = Encoding.UTF8.GetBytes(jsonObject);
                await _azureStorageHelper.CreateShareAndFileAsync(fileName, byteObject, ShareSyllabuses, DirectorySyllabuses);
            }
        }

        private async Task<List<CourseEvaluation>> ReadFile(string path, string term)
        {
            var courses = new List<CourseEvaluation>();
            var fileNameFormat = $"{term}*{FILE_EXTENSION}";

            // Read data from all specific files available 
            if (Debugger.IsAttached)
            {
                // Read all files from local share folder; if debugging
                foreach (var f in Directory.GetFiles(path, fileNameFormat, SearchOption.TopDirectoryOnly))
                {
                    using (StreamReader reader = File.OpenText(f))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        var courseData = (CourseEvaluation)serializer.Deserialize(reader, typeof(CourseEvaluation));
                        courses.Add(courseData);
                    }
                }
            }
            else
            {
                // Read from Azure share; if Release mode
                courses = await _azureStorageHelper.ReadShareFilesAsync(fileNameFormat, ShareSyllabuses, DirectorySyllabuses);
            }

            return courses;
        }

        private string FormatFileName(string course, string session)
        {
            string course1 = RegexFindEmptyChar.Replace(course, "");
            string session1 = RegexFindEmptyChar.Replace(session, "");

            return string.Format($"Course_{session1}_{course1}");
        }
    }
}
