namespace MailJobServer.Models
{
    public class JobInfo
    {
        public string JobId { get; set; }
        public string DataJson { get; set; }
        public string ZipPath { get; set; }
        public DateTime Created { get; set; }
    }

}
