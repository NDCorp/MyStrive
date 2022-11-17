using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Files.Shares;
using ExtractorApi.Controllers;
using Newtonsoft.Json;
using ExtractorApi.Model;
using System.Text;
using System.IO;
using static System.Net.WebRequestMethods;

namespace ExtractorApi.Helper
{
    public class AzureStorageHelper: IAzureStorageHelper
    {
        private const string STORAGE1 = "Azure:Configuration:AppSettings:Storage1:{0}:value";
        private const string CONNECTION_STRING_SETTING_NBR = "0";
        private const string ACCOUNT_NAME_SETTING_NBR = "1";
        private const string ACCOUNT_KEY_SETTING_NBR = "2";
        private const long MAX_SIZE = 1048;

        private readonly ILogger<AzureStorageHelper> _logger;
        private readonly IConfiguration _configuration;
        public AzureStorageHelper(ILogger<AzureStorageHelper> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task CreateShareAndFileAsync(string fileName, byte[] byteObject, string shareName, string directoryName)
        {
            // Get the connection string from app settings
            string connectionString = _configuration.GetValue<string>(string.Format(STORAGE1, CONNECTION_STRING_SETTING_NBR)); 

            try
            { 
                // Instantiate a ShareClient to manipulate the Azure file share
                ShareClient azShare = new ShareClient(connectionString, shareName);
                await azShare.CreateIfNotExistsAsync();   // Create the share if it doesn't already exist

                if (await azShare.ExistsAsync())
                {
                    // Get a reference to the directory and create it, if it doesn't already exist
                    ShareDirectoryClient directory = azShare.GetDirectoryClient(directoryName);
                    await directory.CreateIfNotExistsAsync();

                    if (await directory.ExistsAsync())
                    {
                        // Get a reference to a file object
                        ShareFileClient file = await directory.CreateFileAsync(fileName, byteObject.LongLength); 

                        if (await file.ExistsAsync())
                        {
                            // Update file content in Azure share
                            using (var stream = new MemoryStream(byteObject))
                            {
                                Azure.HttpRange httpRange = new Azure.HttpRange(0, stream.Length);
                                await file.UploadRangeAsync(httpRange, stream);
                                await stream.FlushAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateShareAndFileAsync failed {fileName} --> {ex}");
                throw;
            }
        }

        public async Task<List<CourseEvaluation>> ReadShareFilesAsync(string fileNameFormat, string shareName, string directoryName)
        {
            var courseDataList = new List<CourseEvaluation>();
            // Get the connection string from app settings
            string connectionString = _configuration.GetValue<string>(string.Format(STORAGE1, CONNECTION_STRING_SETTING_NBR));

            try
            {
                // Instantiate a ShareClient to manipulate the Azure file share
                ShareClient azShare = new ShareClient(connectionString, shareName);

                if (await azShare.ExistsAsync())
                {
                    // Get a reference to the directory and create it, if it doesn't already exist
                    ShareDirectoryClient directory = azShare.GetDirectoryClient(directoryName);

                    if (await directory.ExistsAsync())
                    {
                        var shareFiles = directory.GetFilesAndDirectories("Course_");

                        // Read all files from Azure share folder
                        if (shareFiles != null) { 
                            foreach (var f in shareFiles)
                            {
                                if (!f.IsDirectory) { 
                                    // Get a reference to a file object
                                    ShareFileClient file = directory.GetFileClient(f.Name);

                                    if (await file.ExistsAsync())
                                    {
                                        // Read file content from Azure share and add the object to List
                                        using (var stream = file.OpenReadAsync().Result)
                                        using (var reader = new StreamReader(stream))
                                        {
                                            var content = await reader.ReadToEndAsync();
                                            var courseData = JsonConvert.DeserializeObject<CourseEvaluation>(content);
                                            courseDataList.Add(courseData);
                                            await stream.FlushAsync();
                                        }

                                    }
                                }
                            }
                        }
                        
                    }
                }

                return courseDataList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ReadShareFilesAsync failed {fileNameFormat} --> {ex}");
                throw;
            }
        }

    }
}
