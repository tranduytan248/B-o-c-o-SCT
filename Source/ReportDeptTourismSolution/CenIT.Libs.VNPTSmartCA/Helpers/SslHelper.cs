using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TSFramework.App.Processors;

namespace CenIT.Libs.VNPTSmartCA.Helpers
{
    /// <summary>
    /// </summary>
    public class SslHelper
    {
        /// <summary>
        ///     Validate server ssl certificate
        ///     Do real validate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="policyErrors"></param>
        /// <returns></returns>
        public static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain,
            SslPolicyErrors policyErrors)
        {
            AppProcessor.Logger.Message("SslHelper: Server certificate=" + cert.Subject);
            return true;
        }
    }
}