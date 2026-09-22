namespace TSFramework.Core.Members.WebRequest
{
    public class ReturnObjectModel
    {
        public int status { get; set; }
        public string CodeError { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}