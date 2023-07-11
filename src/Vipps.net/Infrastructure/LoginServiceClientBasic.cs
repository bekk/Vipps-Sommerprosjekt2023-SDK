using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vipps.net.Helpers;

namespace Vipps.net.Infrastructure
{
    internal sealed class LoginServiceClientBasic : BaseServiceClient
    {
        internal LoginServiceClientBasic(IVippsHttpClient vippsHttpClient)
            : base(vippsHttpClient) { }

        protected override async Task<Dictionary<string, string>> GetHeaders(
            CancellationToken cancellationToken 
        )  
        {
            return await Task.FromResult(
                new Dictionary<string, string>
                {
                    {Constants.HeaderNameAuthorization, $"Basic {EncodeCredentials(VippsConfiguration.ClientId, VippsConfiguration.ClientSecret)}"}
                }
            );
        }
        private static string EncodeCredentials(string clientId, string clientSecret)
        {
            string credentials = clientId + ":" + clientSecret;
            byte[] credentialsBytes = Encoding.UTF8.GetBytes(credentials);
            string clientAuthorization = Convert.ToBase64String(credentialsBytes);

            return clientAuthorization;
        }
    }
}
