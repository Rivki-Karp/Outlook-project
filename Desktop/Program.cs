using Desktop.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var jobService = new JobService();
        var outlookService = new OutlookService();

        Console.WriteLine("Listening for new jobs...");

        while (true)
        {
            var job = await jobService.GetNextJobAsync();

            if (job != null && !string.IsNullOrWhiteSpace(job.JobId))
            {
                // הורדת רק הקבצים שהמשתמש העלה
                job.Files = await jobService.DownloadAndExtractJobFilesAsync(job);

                if (job.Files.Count > 0)
                {
                    outlookService.CreateDraftEmail(
                        recipient: job.Email,
                        subject: $"New Job from {job.FullName}",
                        body: job.Notes ?? "",
                        attachments: job.Files
                    );
                }

                await jobService.CompleteJobAsync(job.JobId);
            }

            // בדיקה כל 5 שניות
            await Task.Delay(5000);
        }
    }
}
