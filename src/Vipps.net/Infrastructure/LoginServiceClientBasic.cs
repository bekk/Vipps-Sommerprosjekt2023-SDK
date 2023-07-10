using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vipps.net.Helpers;
using Vipps.net.Models.Login;

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
                    {Constants.HeaderNameAuthorization, $"Basic {EncodeCredentials(Environment.GetEnvironmentVariable("CLIENT_ID"), Environment.GetEnvironmentVariable("CLIENT_SECRET"))}"}
                }
            );
        }
        static string EncodeCredentials(string client_id, string client_secret)
        {
            string credentials = client_id + ":" + client_secret;
            byte[] credentialsBytes = Encoding.UTF8.GetBytes(credentials);
            string client_authorization = Convert.ToBase64String(credentialsBytes);

            return client_authorization;
        }
    }
}
