namespace CenIT.ReportTourism.Core.SmtpConn
{
    internal abstract class SmtpConnectorBase
    {
        public const string EOF = "\r\n";

        protected SmtpConnectorBase(string smtpServerAddress, int port)
        {
            SmtpServerAddress = smtpServerAddress;
            Port = port;
        }

        private string SmtpServerAddress { get; }
        private int Port { get; }

        public abstract bool CheckResponse(int expectedCode);
        public abstract void SendData(string data);
    }
}