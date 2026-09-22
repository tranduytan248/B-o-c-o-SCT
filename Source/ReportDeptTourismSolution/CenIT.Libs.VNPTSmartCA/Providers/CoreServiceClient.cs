using System;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Text;
using CenIT.Libs.VNPTSmartCA.Helpers;
using CenIT.Libs.VNPTSmartCA.Models;
using Newtonsoft.Json;
using RestSharp;
using TSFramework.App.Processors;

namespace CenIT.Libs.VNPTSmartCA.Providers
{
    /// <summary>
    /// Gateway API request handler
    /// </summary>
    public class CoreServiceClient
    {
        /// <summary>
        /// Request for protected resource with access_token
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static ResponseMessage Query(RequestMessage req, string accessToken)
        {
            // Verify service certificate. 
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(SslHelper.ValidateRemoteCertificate);

            // Create RestClient instance and add client certificate
            var serviceUri = ConfigurationManager.AppSettings["SERVICE_URL"];
            RestClient client = new RestClient(serviceUri);

            RestRequest request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBody(req);
            request.AddHeader("Authorization", $"Bearer {accessToken}");

            IRestResponse response = null;
            try
            {
                response = client.Execute(request);
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }

            if (response == null || response.ErrorException != null)
            {
                AppProcessor.Logger.Message("Service return null response");
                return null;
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                AppProcessor.Logger.Message($"Status code={response.StatusCode}. Status content: {response.Content}");
                return null;
            }

            var respContent = response.Content;
            try
            {
                return JsonConvert.DeserializeObject<ResponseMessage>(respContent);
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                return null;
            }
        }

        public static String Query(object req, string serviceUri, string accessToken)
        {
            // Verify service certificate. 
            ServicePointManager.ServerCertificateValidationCallback
                += new RemoteCertificateValidationCallback(SslHelper.ValidateRemoteCertificate);

            RestClient client = new RestClient(serviceUri);
            RestRequest request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBody(req);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            IRestResponse response = null;
            try
            {
                response = client.Execute(request);
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }

            if (response == null || response.ErrorException != null)
            {
                AppProcessor.Logger.Message("Service return null response");
                return null;
            }

            if (response.StatusCode == HttpStatusCode.OK) return response.Content;
            AppProcessor.Logger.Message($"Status code={response.StatusCode}. Status content: {response.Content}");
            return null;

        }

        /// <summary>
        /// Request new access_token from long live refresh_token
        /// </summary>
        /// <param name="refreshToken">refresh_token from previous access_token request</param>
        /// <param name="newRefreshToken">return new refresh_token</param>
        /// <returns>new access_token</returns>
        public static string RefreshToken(string refreshToken, out string newRefreshToken)
        {
            newRefreshToken = "";
            // Verify service certificate. 
            ServicePointManager.ServerCertificateValidationCallback
                += new RemoteCertificateValidationCallback(SslHelper.ValidateRemoteCertificate);

            // Create RestClient instance and add client certificate
            var tokenUri = ConfigurationManager.AppSettings["SERVICE_GET_TOKENURL"];
            RestClient client = new RestClient(tokenUri);

            // Token request body
            RestRequest request = new RestRequest(Method.POST);
            var req = new TokenBodyModel
            {
                GrantType = "refresh_token",
                ClientID = ConfigurationManager.AppSettings["APP_ID"],
                ClientSecret = ConfigurationManager.AppSettings["APP_SECRET"],
            };
            var param = JsonConvert.SerializeObject(req);
            AppProcessor.Logger.Message("access_token request:" + param);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded",
                $"grant_type={req.GrantType}"
                + $"&client_id={req.ClientID}"
                + $"&client_secret={req.ClientSecret}"
                + $"&refresh_token={refreshToken}",
                ParameterType.RequestBody);

            IRestResponse response = null;
            try
            {
                response = client.Execute(request);

            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }
            if (response == null || response.ErrorException != null)
            {
                AppProcessor.Logger.Message("Service return null response");
                return null;
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                AppProcessor.Logger.Message($"Status code={response.StatusCode}. Status content: {response.Content}");
                return null;
            }

            var respContent = response.Content;
            try
            {
                var resp = JsonConvert.DeserializeObject<TokenResponseModel>(respContent);
                AppProcessor.Logger.Message("access_token:" + resp.AccessToken);
                AppProcessor.Logger.Message($"refresh_token: {resp.RefreshToken}");
                newRefreshToken = resp.RefreshToken;
                return resp.AccessToken;
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Resource Owner Password grant.
        /// Send user email/password to get access_token
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string GetAccessToken(string email, string pass, out string refresh_token)
        {
            refresh_token = "";
            // Verify service certificate. 
            ServicePointManager.ServerCertificateValidationCallback
                += new RemoteCertificateValidationCallback(SslHelper.ValidateRemoteCertificate);

            // Create RestClient instance and add client certificate
            var tokenUri = ConfigurationManager.AppSettings["SERVICE_GET_TOKENURL"];
            RestClient client = new RestClient(tokenUri);

            // Token request body
            RestRequest request = new RestRequest(Method.POST);
            var req = new TokenBodyModel
            {
                GrantType = "password",
                Username = email,
                Password = pass,
                ClientID = ConfigurationManager.AppSettings["APP_ID"],
                ClientSecret = ConfigurationManager.AppSettings["APP_SECRET"],
            };
            var param = JsonConvert.SerializeObject(req);
            AppProcessor.Logger.Message("access_token request:" + param);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type={req.GrantType}" +
                $"&username={req.Username}&password={req.Password}&client_id={req.ClientID}" +
                $"&client_secret={req.ClientSecret}", ParameterType.RequestBody);
            // End: Token request body

            IRestResponse response = null;
            try
            {
                response = client.Execute(request);
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }

            if (response == null || response.ErrorException != null)
            {
                AppProcessor.Logger.Message("Service return null response");
                return null;
            }
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
            var result = encoding.GetString(response.RawBytes);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                AppProcessor.Logger.Message($"Status code={response.StatusCode}. Status content: {response.Content}");
                return null;
            }

            var respContent = response.Content;
            try
            {
                var resp = JsonConvert.DeserializeObject<TokenResponseModel>(respContent);
                AppProcessor.Logger.Message("access_token:" + resp.AccessToken);
                AppProcessor.Logger.Message($"refresh_token: {resp.RefreshToken}");
                refresh_token = resp.RefreshToken;
                return resp.AccessToken;
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                return null;
            }
        }

    }
}