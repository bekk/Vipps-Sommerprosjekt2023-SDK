using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vipps.net.Helpers;

namespace Vipps.net.Infrastructure
{
    public sealed class LoginServiceClientBasic : BaseServiceClient
    {
        private readonly IVippsConfigurationProvider _vippsConfigurationProvider;

        internal LoginServiceClientBasic(IVippsHttpClient vippsHttpClient,
            IVippsConfigurationProvider vippsConfigurationProvider)
            : base(vippsHttpClient)
        {
            _vippsConfigurationProvider = vippsConfigurationProvider; 
        }

        protected override async Task<Dictionary<string, string>> GetHeaders(
            CancellationToken cancellationToken 
        )
        {
            var options = _vippsConfigurationProvider.GetConfiguration(); 
            return await Task.FromResult(
                new Dictionary<string, string>
                {
                    {Constants.HeaderNameAuthorization, $"Basic {EncodeCredentials(options.ClientId, options.ClientSecret)}"}
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
