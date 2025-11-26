using System.Collections.Generic;

namespace Desktop.Models
{
    public class JobResponse
    {
        public string JobId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;

        // רשימה של קבצים שהמשתמש העלה
        public List<string> Files { get; set; } = new List<string>();

        // כתובת ZIP שהשרת מחזיר
        public string ZipUrl { get; set; } = string.Empty;
    }
}
