using MailJobServer.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly string tempPath = Path.Combine("temp", "jobs");

    public JobsController()
    {
        Directory.CreateDirectory(tempPath);
    }

    [HttpPost("upload")]
    [SwaggerOperation(Summary = "Upload form data + files and create a job")]
    public async Task<IActionResult> UploadJob(
    [FromForm] JobRequest request,
    [FromForm] List<IFormFile> files)
    {
        string jobId = Guid.NewGuid().ToString();
        string jobFolder = Path.Combine(tempPath, jobId);
        Directory.CreateDirectory(jobFolder);

        // שמירת הנתונים כ־JSON
        string jsonPath = Path.Combine(jobFolder, "data.json");
        await System.IO.File.WriteAllTextAsync(
            jsonPath,
            System.Text.Json.JsonSerializer.Serialize(request)
        );

        // שמירת הקבצים
        foreach (var file in files)
        {
            string filePath = Path.Combine(jobFolder, file.FileName);
            using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);
            await stream.FlushAsync(); // לוודא שהקובץ סגור לגמרי
        }

        // יצירת ZIP בתיקייה נפרדת כדי למנוע שימוש בקבצים פתוחים
        string zipPath = Path.Combine(tempPath, $"{jobId}.zip");
        System.IO.Compression.ZipFile.CreateFromDirectory(jobFolder, zipPath);

        return Ok(new
        {
            jobId,
            zipUrl = $"https://localhost:7094/api/jobs/zip/{jobId}"
        });
    }

    [HttpGet("next")]
    [SwaggerOperation(Summary = "Desktop pulls the next job available")]
    public IActionResult GetNextJob()
    {
        var jobDirs = Directory.GetDirectories(tempPath);

        if (jobDirs.Length == 0)
            return NoContent();

        string folder = jobDirs[0];
        string jobId = Path.GetFileName(folder);

        string jsonPath = Path.Combine(folder, "data.json");

        if (!System.IO.File.Exists(jsonPath))
            return BadRequest("Missing data.json file");

        var json = System.IO.File.ReadAllText(jsonPath);
        var jobData = System.Text.Json.JsonSerializer.Deserialize<JobRequest>(json);

        return Ok(new
        {
            JobId = jobId,
            Email = jobData.Email,
            FullName = jobData.FullName,
            Notes = jobData.Notes,
            ZipUrl = $"https://localhost:7094/api/jobs/zip/{jobId}"
        });
    }


    [HttpGet("zip/{jobId}")]
    [SwaggerOperation(Summary = "Download job ZIP")]
    public IActionResult DownloadZip(string jobId)
    {
        // צור נתיב מוחלט
        string zipPath = Path.Combine(Directory.GetCurrentDirectory(), tempPath, $"{jobId}.zip");

        if (!System.IO.File.Exists(zipPath))
            return NotFound();

        return PhysicalFile(zipPath, "application/zip", "job.zip");
    }



    [HttpPost("complete/{jobId}")]
    [SwaggerOperation(Summary = "Desktop notifies job is completed")]
    public IActionResult Complete(string jobId)
    {
        string folder = Path.Combine(tempPath, jobId);

        if (Directory.Exists(folder))
            Directory.Delete(folder, true);

        return Ok();
    }
}
