using System.Collections.Generic;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace Desktop.Services
{
    public class OutlookService
    {
        private readonly Outlook.Application _outlookApp;

        public OutlookService()
        {
            _outlookApp = new Outlook.Application();
        }

        public void CreateDraftEmail(string recipient, string subject, string body, List<string> attachments)
        {
            Outlook.MailItem mail = (Outlook.MailItem)_outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
            mail.To = recipient;
            mail.Subject = subject;
            mail.Body = body;

            foreach (var attachment in attachments)
            {
                mail.Attachments.Add(attachment);
            }

            mail.Save();    // שומר כטיוטה
            mail.Display(); // פותח את הטיוטה ב-Outlook
        }
    }
}
