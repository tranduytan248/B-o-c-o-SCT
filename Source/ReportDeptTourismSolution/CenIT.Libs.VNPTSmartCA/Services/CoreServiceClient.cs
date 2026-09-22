using System;
using System.IO;
using System.Net;
using RestSharp;

namespace CenIT.Libs.VNPTSmartCA.Services
{
    public static class AuthorizationType
    {
        public const string Basic = "Basic";
        public const string Bearer = "Bearer";
    }

    public static class ServiceType
    {
        public const string AuthToken = "{0}/auth/token";
        public const string UserInfo = "{0}/identityapi/userinfo/info";
        public const string ListCredentials = "{0}/csc/credentials/list";
        public const string CredentialInfo = "{0}/csc/credentials/info";
        public const string SignHash = "{0}/csc/signature/signhash";
        public const string Sign = "{0}/csc/signature/sign";
        public const string GetTranInfo = "{0}/csc/credentials/gettraninfo";
    }

    public static class ConstContentTypes
    {
        public const string TypeJson = "application/json";
        public const string TypeXml = "application/xml";
        public const string TypeFormUrlEncoded = "application/x-www-form-urlencoded";
    }

    public static class CTSType
    {
        public const string None = "none";
        public const string Single = "single";
        public const string Chain = "chain";
    }

    public static class CoreServiceClient
    {
        public static string Request(string pzUrlRequest, string pzData, string authorizationType,
            string pzAuthorization, string pzMethod, string pzContentType, out Exception ex)
        {
            ex = null;
            try
            {
                var httpWebRequest = (HttpWebRequest) WebRequest.Create(pzUrlRequest);
                httpWebRequest.ContentType = pzContentType;
                httpWebRequest.Method = pzMethod;
                httpWebRequest.KeepAlive = false;
                if (!string.IsNullOrEmpty(pzAuthorization))
                    httpWebRequest.Headers.Add("Authorization", $"{authorizationType} {pzAuthorization}");

                httpWebRequest.Proxy = new WebProxy(); //no proxy

                if (!string.IsNullOrEmpty(pzData))
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        var json = pzData;

                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                InitiateSslTrust(); //bypass SSL
                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                var dataResponse = httpResponse.GetResponseStream();
                if (dataResponse == null) return null;
                string result;
                using (var streamReader = new StreamReader(dataResponse))
                {
                    result = streamReader.ReadToEnd();
                }

                return result;
            }
            catch (Exception err)
            {
                ex = err;
                return null;
            }
        }

        public static string Query(string pzUrlRequest, string pzData, string authorizationType, string pzAuthorization,
            Method pzMethod, DataFormat? dataFormat, string contentType, out Exception ex)
        {
            ex = null;

            InitiateSslTrust();
            var client = new RestClient(pzUrlRequest);
            var request = new RestRequest(pzMethod);
            request.AddHeader("content-type", contentType);

            if (dataFormat != null)
                request.RequestFormat = dataFormat.Value;
            if (dataFormat == DataFormat.Json)
                request.AddJsonBody(pzData);
            else
                request.AddParameter(contentType, pzData, ParameterType.RequestBody);

            if (!string.IsNullOrEmpty(authorizationType) && !string.IsNullOrEmpty(pzAuthorization))
                request.AddHeader("Authorization", $"{authorizationType} {pzAuthorization}");

            IRestResponse response;
            try
            {
                response = client.Execute(request);
            }
            catch (Exception err)
            {
                ex = err;
                return null;
            }

            if (response == null || response.ErrorException != null)
            {
                ex = response?.ErrorException;
                //AppProcessor.Logger.Message("Service return null response");
                return null;
            }

            if (response.StatusCode == HttpStatusCode.OK) return response.Content;
            //AppProcessor.Logger.Message($"Status code={response.StatusCode}. Status content: {response.Content}");
            ex = new Exception(response.Content);

            return null;
        }

        public static string Query(string pzUrlRequest, object pzData, string authorizationType, string pzAuthorization,
            Method pzMethod, DataFormat? dataFormat, out Exception ex)
        {
            ex = null;

            InitiateSslTrust();
            var client = new RestClient(pzUrlRequest);
            var request = new RestRequest(pzMethod);

            if (dataFormat != null)
                request.RequestFormat = dataFormat.Value;

            if (dataFormat == DataFormat.Json)
                request.AddJsonBody(pzData);

            if (!string.IsNullOrEmpty(authorizationType) && !string.IsNullOrEmpty(pzAuthorization))
                request.AddHeader("Authorization", $"{authorizationType} {pzAuthorization}");

            IRestResponse response;
            try
            {
                response = client.Execute(request);
            }
            catch (Exception err)
            {
                ex = err;
                return null;
            }

            if (response == null || response.ErrorException != null)
            {
                ex = response?.ErrorException;
                //AppProcessor.Logger.Message("Service return null response");
                return null;
            }

            if (response.StatusCode == HttpStatusCode.OK) return response.Content;
            //AppProcessor.Logger.Message($"Status code={response.StatusCode}. Status content: {response.Content}");
            ex = new Exception(response.Content);

            return null;
        }

        private static void InitiateSslTrust()
        {
            //ServicePointManager.ServerCertificateValidationCallback
            //    += new RemoteCertificateValidationCallback(SslHelper.ValidateRemoteCertificate);

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
    }
}