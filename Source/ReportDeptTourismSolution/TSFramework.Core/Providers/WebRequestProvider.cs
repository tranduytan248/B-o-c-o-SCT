using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace TSFramework.Core.Providers
{
    public class WebRequestProvider
    {
        public static T Get<T>(string url, Dictionary<string, string> postParams,
            Dictionary<string, string> headerParams = null)
        {
            return MakeRequest<T>("GET", url, headerParams, postParams);
        }

        public static T Post<T>(string url, Dictionary<string, string> postParams,
            Dictionary<string, string> headerParams = null)
        {
            return MakeRequest<T>("POST", url, headerParams, postParams);
        }

        private static T MakeRequest<T>(string httpMethod, string url, Dictionary<string, string> headerParams = null,
            Dictionary<string, string> postParams = null)
        {
            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod), url);

                if (headerParams != null)
                    foreach (var entry in headerParams)
                        requestMessage.Headers.Add(entry.Key, entry.Value);

                if (postParams != null)
                    requestMessage.Content =
                        new FormUrlEncodedContent(
                            postParams); // This is where your content gets added to the request body


                var response = client.SendAsync(requestMessage).Result;

                var apiResponse = response.Content.ReadAsStringAsync().Result;
                // Attempt to deserialise the reponse to the desired type, otherwise throw an expetion with the response from the api.
                if (apiResponse != "")
                    return JsonConvert.DeserializeObject<T>(apiResponse);
                return default(T);
            }
        }
    }
}