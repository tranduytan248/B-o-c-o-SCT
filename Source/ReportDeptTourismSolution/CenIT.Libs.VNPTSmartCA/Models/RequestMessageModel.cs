using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestMessage
    {
        public string RequestID { get; set; }
        public string ServiceID { get; set; }
        public string FunctionName { get; set; }

        // Complex property in request message. See API document to custom object for each request type
        public object Parameter { get; set; }
    }

    public class RequestCredentialModel
    {

    }
    public class ReponseCredentialModel
    {
        [JsonProperty("credentialId")]
        public string CredentialId { get; set; }
        [JsonProperty("certificates")]
        public string Certificates { get; set; }
        [JsonProperty("certInfo")]
        public bool CertInfo { get; set; }
        [JsonProperty("authInfo")]
        public bool AuthInfo { get; set; }
    }

    public class TranInfoSmartCAResp
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("codeDesc")]
        public string CodeDesc { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("content")]
        public TranInfoSmartCARespContent Content { get; set; }
    }

    public class TranInfoSmartCARespContent
    {
        [JsonProperty("refTranId")]
        public string RefTranId { get; set; }
        [JsonProperty("tranId")]
        public string TranId { get; set; }
        [JsonProperty("sub")]
        public string Sub { get; set; }
        [JsonProperty("credentialId")]
        public string CredentialId { get; set; }
        [JsonProperty("tranType")]
        public int TranType { get; set; }
        [JsonProperty("tranTypeDesc")]
        public string TranTypeDesc { get; set; }
        [JsonProperty("tranStatus")]
        public int TranStatus { get; set; }
        [JsonProperty("transStatusDesc")]
        public string TransStatusDesc { get; set; }
        [JsonProperty("reqTime")]
        public DateTime? ReqTime { get; set; }
        [JsonProperty("documents")]
        public List<DocumentResp> Documents { get; set; }

    }
    public class DocumentResp
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("size")]
        public string Size { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("sig")]
        public string Sig { get; set; }
        [JsonProperty("signature")]
        public string Signature { get; set; }
        [JsonProperty("dataSigned")]
        public string DataSigned { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }

    }

    /// <summary>
    /// Property 'Parameter' in request message
    /// </summary>
    public class Parameter
    {
        public string Email { get; set; }
    }

    // Certificate ----------------------------------------------------
    public class CertParameter : Parameter
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class CertResponse
    {
        public int Count { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public List<Certificate> Items { get; set; }
    }

    public class CredentialSmartCAResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("codeDesc")]
        public string CodeDesc { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("content")]
        public List<string> Content { get; set; }
    }

    public class SignHashSmartCAResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("codeDesc")]
        public string CodeDesc { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("content")]
        public ContenSignHash Content { get; set; }
    }

    public class ContenSignHash
    {
        [JsonProperty("tranId")]
        public string TranId { get; set; }
    }

    public class CertificateSmartCAResponse
    {
        [JsonProperty("cert")]
        public CertRes Cert { get; set; }
        [JsonProperty("key")]
        public KeyRes Key { get; set; }
        [JsonProperty("authMode")]
        public string AuthMode { get; set; }
        [JsonProperty("scal")]
        public string Scal { get; set; }
        [JsonProperty("mutisign")]
        public string Mutisign { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class SignHashSmartCAReq
    {
        [JsonProperty("credentialId")]
        public string CredentialId { get; set; }
        [JsonProperty("refTranId")]
        public string RefTranId { get; set; }
        [JsonProperty("notifyUrl")]
        public string NotifyUrl { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("datas")]
        public List<DataSignHash> Datas { get; set; }

    }

    public class SignSmartCAReq
    {
        [JsonProperty("credentialId")]
        public string CredentialId { get; set; }
        [JsonProperty("refTranId")]
        public string RefTranId { get; set; }
        [JsonProperty("notifyUrl")]
        public string NotifyUrl { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("datas")]
        public List<DataSign> Datas { get; set; }
    }
    public class DataSign
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("dataBase64")]
        public string DataBase64 { get; set; }
    }

    public class DataSignHash
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
    }

    public class CertRes
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }
        [JsonProperty("subjectDN")]
        public string SubjectDn { get; set; }
        [JsonProperty("issuerDN")]
        public string IssuerDn { get; set; }
        [JsonProperty("certificates")]
        public List<string> Certificates { get; set; }
        [JsonProperty("validFrom")]
        public string ValidFrom { get; set; }
        [JsonProperty("validTo")]
        public string ValidTo { get; set; }
    }

    public class KeyRes
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("alg")]
        public List<string> Alg { get; set; }
        [JsonProperty("len")]
        public int Len { get; set; }
    }


    public class Certificate
    {
        public string ID { get; set; }
        public string CertBase64 { get; set; }
        // More properties, see json response
    }

    // ---------------------------------------------------------------

    // Signature -----------------------------------------------------
    public class SignParameter
    {
        public string CertID { get; set; }
        public string ServiceGroupID { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public string DataBase64 { get; set; }
    }
    public class SignResponse
    {
        public string TranID { get; set; }
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public string SignedData { get; set; }
    }
    // ---------------------------------------------------------------

    // Verify response -----------------------------------------------
    public class VerifyResultModel
    {
        [JsonProperty("TranID")]
        public string TranID { get; set; }
        [JsonProperty("status")]
        public bool Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("signatures")]
        public List<SignServerVerifyResultModel> Signatures { get; set; }
    }
    public class SignServerVerifyResultModel
    {
        [JsonProperty("signingTime")]
        public string SigningTime { get; set; }
        [JsonProperty("signatureStatus")]
        public bool SignatureStatus { get; set; }
        [JsonProperty("certStatus")]
        public string CertStatus { get; set; }
        [JsonProperty("certificate")]
        public string Certificate { get; set; }
        [JsonProperty("signatureIndex")]
        public int SignatureIndex { get; set; }
        [JsonProperty("code")]
        public int Code { get; set; }
    }
    // ---------------------------------------------------------------
}
