namespace TSFramework.Core.Members.WebRequest
{
    public class ReturnTokenModel
    {
        public int status { get; set; }
        public string codeError { get; set; }
        public string message { get; set; }
        public string token { get; set; } = "";
        public string userName { get; set; } = "";
        public string fullName { get; set; } = "";
    }
}