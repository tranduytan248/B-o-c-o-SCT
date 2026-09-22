using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.Owin.Security.DataHandler.Encoder;

namespace TSFramework.App.OAuth
{
    public static class AudiencesStore
    {
        public const string DefaultClientId = "099153c2625149bc8ecb3e85e03f0022";
        public const string DefaultClientSecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw";

        public static ConcurrentDictionary<string, Audience> AudiencesList =
            new ConcurrentDictionary<string, Audience>();

        static AudiencesStore()
        {
            AudiencesList.TryAdd(DefaultClientId,
                new Audience
                {
                    ClientId = DefaultClientId,
                    Base64Secret = DefaultClientSecret,
                    Name = "App.API.Audiences"
                });
        }

        public static Audience AddAudience(string name)
        {
            var clientId = Guid.NewGuid().ToString("N");

            var key = new byte[32];
            RandomNumberGenerator.Create().GetBytes(key);
            var base64Secret = TextEncodings.Base64Url.Encode(key);

            var newAudience = new Audience {ClientId = clientId, Base64Secret = base64Secret, Name = name};
            AudiencesList.TryAdd(clientId, newAudience);
            return newAudience;
        }

        public static Audience FindAudience(string clientId)
        {
            Audience audience;
            if (AudiencesList.TryGetValue(clientId, out audience)) return audience;
            return null;
        }
    }
}