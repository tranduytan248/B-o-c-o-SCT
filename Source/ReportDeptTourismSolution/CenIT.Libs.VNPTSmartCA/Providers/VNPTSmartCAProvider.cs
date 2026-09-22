using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using CenIT.Libs.VNPTSmartCA.Helpers;
using CenIT.Libs.VNPTSmartCA.Models;
using CenIT.Libs.VNPTSmartCA.Models.Requests;
using CenIT.Libs.VNPTSmartCA.Models.Responses;
using CenIT.Libs.VNPTSmartCA.Services;
using Newtonsoft.Json;
using RestSharp;
using TSFramework.App.Processors;
using VnptHashSignatures.Common;
using VnptHashSignatures.Interface;

namespace CenIT.Libs.VNPTSmartCA.Providers
{
    public static class VNPTSmartCAProvider
    {
        /// <summary>
        ///     Resource Owner Password grant.
        ///     Send user email/password to get access_token
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string AuthToken(string userName, string password, out string refreshToken)
        {
            refreshToken = "";
            Exception err;
            var serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            var tokenUri = string.Format(ServiceType.AuthToken, serviceUrl);
            var tokenBody = new TokenBodyModel
            {
                GrantType = "password",
                Username = userName,
                Password = password,
                ClientID = ConfigurationManager.AppSettings["APP_ID"],
                ClientSecret = ConfigurationManager.AppSettings["APP_SECRET"]
            };
            var dataFormEncode = FormUrlEncodedHelper.GenFormEncode(tokenBody, true, typeof(JsonPropertyAttribute));
            var requestParam = string.Join("&", dataFormEncode.Select(kvp => $"{kvp.Key}={kvp.Value ?? ""}"));
            //var dataToken = CoreServiceClient.Request(tokenUri, requestParam, String.Empty, string.Empty,
            //    WebRequestMethods.Http.Post, ConstContentTypes.TypeFormUrlEncoded, out err);

            var dataToken = CoreServiceClient.Query(tokenUri, requestParam, string.Empty, string.Empty,
                Method.POST, null, ConstContentTypes.TypeFormUrlEncoded, out err);

            if (err != null)
            {
                AppProcessor.Logger.Error(err);
                return null;
            }

            try
            {
                var resp = JsonConvert.DeserializeObject<TokenResponseModel>(dataToken);
                AppProcessor.Logger.Message($"access_token: {resp.AccessToken}");
                AppProcessor.Logger.Message($"refresh_token: {resp.RefreshToken}");
                refreshToken = resp.RefreshToken;
                return resp.AccessToken;
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
                return null;
            }
        }

        /// <summary>
        ///     Request new access_token from long live refresh_token
        /// </summary>
        /// <param name="refreshToken">refresh_token from previous access_token request</param>
        /// <param name="newRefreshToken">return new refresh_token</param>
        /// <returns>new access_token</returns>
        public static string RefreshToken(string refreshToken, out string newRefreshToken)
        {
            newRefreshToken = "";
            Exception err;
            var serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            var tokenUri = string.Format(ServiceType.AuthToken, serviceUrl);
            var tokenBody = new RefreshTokenBodyModel
            {
                GrantType = "refresh_token",
                ClientID = ConfigurationManager.AppSettings["APP_ID"],
                ClientSecret = ConfigurationManager.AppSettings["APP_SECRET"],
                RefreshToken = refreshToken
            };
            var dataFormEncode = FormUrlEncodedHelper.GenFormEncode(tokenBody, true, typeof(JsonPropertyAttribute));
            var requestParam = string.Join("&", dataFormEncode.Select(kvp => $"{kvp.Key}={kvp.Value ?? ""}"));
            var dataToken = CoreServiceClient.Request(tokenUri, requestParam, string.Empty, string.Empty,
                WebRequestMethods.Http.Post, ConstContentTypes.TypeFormUrlEncoded, out err);

            if (err != null)
            {
                AppProcessor.Logger.Error(err);
                return null;
            }

            try
            {
                var resp = JsonConvert.DeserializeObject<TokenResponseModel>(dataToken);
                AppProcessor.Logger.Message($"access_token: {resp.AccessToken}");
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
        ///     Request Get UserInfo via access token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>class UserInfoModel</returns>
        public static UserInfoModel GetUserInfo(string accessToken)
        {
            Exception err;
            var serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            var userInfoUri = string.Format(ServiceType.UserInfo, serviceUrl);
            var requestCredential = new RequestUserInfoModel();

            //var dataResponse = CoreServiceClient.Request(tokenUri, "{}", "Bearer", accessToken,
            //    WebRequestMethods.Http.Post, ConstContentTypes.TypeJson, out err);

            var dataResponse = CoreServiceClient.Query(userInfoUri, requestCredential, AuthorizationType.Bearer,
                accessToken,
                Method.POST, DataFormat.Json, out err);

            if (err != null)
            {
                AppProcessor.Logger.Error(err);
                return null;
            }

            try
            {
                var resp = JsonConvert.DeserializeObject<ResponseUserInfoModel>(dataResponse);
                AppProcessor.Logger.Message($"accType: {resp.Content.AccType}");
                AppProcessor.Logger.Message($"fullName: {resp.Content.FullName}");
                AppProcessor.Logger.Message($"email: {resp.Content.Email}");
                AppProcessor.Logger.Message($"uid: {resp.Content.Uid}");
                return resp.Content;
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        ///     Get list Credentials
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static List<string> GetListCredentials(string accessToken)
        {
            Exception err;
            var serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            var listCredentialUri = string.Format(ServiceType.ListCredentials, serviceUrl);
            var requestCredential = new RequestCredentialModel();

            var dataResponse = CoreServiceClient.Query(listCredentialUri, requestCredential, AuthorizationType.Bearer,
                accessToken,
                Method.POST, DataFormat.Json, out err);

            if (err != null)
                AppProcessor.Logger.Error(err);
            //return null;
            try
            {
                var resp = JsonConvert.DeserializeObject<ResponseCredentialModel>(dataResponse);
                return resp.Content;
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }

            return null;
        }

        public static ResponseCredentialInfoModel GetCredentialInfo(string accessToken, string credentialId,
            string certificates, bool certInfo, bool authInfo)
        {
            Exception err;
            var serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            var listCredentialUri = string.Format(ServiceType.CredentialInfo, serviceUrl);

            var requestInfoCredential = new RequestCredentialInfoModel
            {
                CredentialId = credentialId,
                Certificates = certificates,
                CertInfo = certInfo,
                AuthInfo = authInfo
            };

            var dataResponse = CoreServiceClient.Query(listCredentialUri, requestInfoCredential,
                AuthorizationType.Bearer, accessToken,
                Method.POST, DataFormat.Json, out err);

            if (err != null)
                AppProcessor.Logger.Error(err);
            //return null;

            try
            {
                var resp = JsonConvert.DeserializeObject<ResponseCredentialInfoModel>(dataResponse);
                return resp;
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }

            return null;
        }

        public static string Sign(string accessToken, string credentialId, string description, string fileName, byte[] unsignData)
        {
            Exception err;
            var serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            var signUri = string.Format(ServiceType.Sign, serviceUrl);

            //Sign Begin            
            var req = new RequestSignSmartCAModel
            {
                CredentialId = credentialId,
                Description = description,
                Datas = new List<DataSign>()
            };

            req.Datas.Add(new DataSign
            {
                Name = fileName,
                DataBase64 = Convert.ToBase64String(unsignData)
            });

            var dataResponse = CoreServiceClient.Query(signUri, req, AuthorizationType.Bearer, accessToken,
                Method.POST, DataFormat.Json, out err);

            if (err != null)
                AppProcessor.Logger.Error(err);

            try
            {
                if (dataResponse == null) return null;
                var resp = JsonConvert.DeserializeObject<ResponseSignHashSmartCAModel>(dataResponse);
                if (resp.Code == 0) return resp.Content.TranId;

                AppProcessor.Logger.Message(dataResponse);
                return null;
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }

            return null;
        }

        public static string SignHash(out IHashSigner signer, string accessToken, string credentialId,
            string certBase64, string description, string fileName, byte[] unsignData, string typeExt,
            string notifyUrl = null)
        {
            signer = HashSignerFactory.GenerateSigner(unsignData, certBase64, null, typeExt,
                MessageDigestAlgorithm.SHA256);

            Exception err;
            var serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            var signHashUri = string.Format(ServiceType.SignHash, serviceUrl);

            //SignHash Begin            
            var req = new RequestSignHashSmartCAModel
            {
                CredentialId = credentialId,
                RefTranId = Guid.NewGuid().ToString(),
                NotifyUrl = notifyUrl,
                Description = description,
                Datas = new List<DataSignHash>()
            };

            var hashValue = signer.GetSecondHashAsBase64();

            req.Datas.Add(new DataSignHash
            {
                Name = fileName,
                Hash = hashValue
            });

            var dataResponse = CoreServiceClient.Query(signHashUri, req, AuthorizationType.Bearer, accessToken,
                Method.POST, DataFormat.Json, out err);

            if (err != null)
                AppProcessor.Logger.Error(err);
            //return null;

            try
            {
                if (dataResponse == null) return null;
                var resp = JsonConvert.DeserializeObject<ResponseSignHashSmartCAModel>(dataResponse);
                if (resp.Code == 0) return resp.Content.TranId;

                return null;
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }

            return null;
        }

        public static ResponseTranInfoSmartCARespContentModel GetTranInfo(string accessToken, string tranId)
        {
            Exception err;
            var serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            var signHashUri = string.Format(ServiceType.GetTranInfo, serviceUrl);
            var req = new ContenSignHash
            {
                TranId = tranId
            };
            var dataResponse = CoreServiceClient.Query(signHashUri, req, AuthorizationType.Bearer, accessToken,
                Method.POST, DataFormat.Json, out err);

            if (err != null) AppProcessor.Logger.Error(err);

            try
            {
                if (dataResponse == null) return null;
                var resp = JsonConvert.DeserializeObject<ResponseTranInfoSmartCAModel>(dataResponse);
                if (resp.Code == 0) return resp.Content;

                return null;
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }

            return null;
        }
    }
}