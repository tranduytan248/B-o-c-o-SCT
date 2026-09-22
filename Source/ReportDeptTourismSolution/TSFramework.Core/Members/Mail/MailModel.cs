using System.Collections.Generic;

namespace TSFramework.Core.Members.Mail
{
    public class MailModel
    {
        public string DisplayNameFrom { get; set; }
        public string From { get; set; }
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; } = false;

        public Dictionary<string, byte[]> DicImgs { get; set; }
    }
}