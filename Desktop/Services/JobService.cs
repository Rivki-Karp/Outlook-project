using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Desktop.Models;

namespace Desktop.Services
{
    public class JobService
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _apiBaseUrl = "https://localhost:7094/api/jobs";

        public async Task<JobResponse?> GetNextJobAsync()
        {
            try
            {
                var response = await _client.GetAsync($"{_apiBaseUrl}/next");

                if (!response.IsSuccessStatusCode)
                    return null;

                var job = await response.Content.ReadFromJsonAsync<JobResponse>();

                if (job == null || string.IsNullOrWhiteSpace(job.JobId))
                    return null;

                return job;
            }
            catch
            {
                return null;
            }
        }

        public async Task CompleteJobAsync(string jobId)
        {
            await _client.PostAsync($"{_apiBaseUrl}/complete/{jobId}", null);
        }

        public async Task<List<string>> DownloadAndExtractJobFilesAsync(JobResponse job)
        {
            var downloadedFiles = new List<string>();
            string tempFolder = Path.Combine(Path.GetTempPath(), job.JobId);

            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);

            Directory.CreateDirectory(tempFolder);

            // הורדת קובץ ZIP מהשרת
            string zipPath = Path.Combine(tempFolder, $"{job.JobId}.zip");
            using (var stream = await _client.GetStreamAsync(job.ZipUrl))
            using (var fileStream = File.Create(zipPath))
            {
                await stream.CopyToAsync(fileStream);
            }

            // חילוץ הקבצים מה-ZIP
            ZipFile.ExtractToDirectory(zipPath, tempFolder);

            // הוספת כל הקבצים למטמון רק אם זה לא data.json או ZIP
            foreach (var file in Directory.GetFiles(tempFolder))
            {
                if (!file.EndsWith(".zip") && !file.EndsWith("data.json"))
                    downloadedFiles.Add(file);
            }

            return downloadedFiles;
        }
    }
}
