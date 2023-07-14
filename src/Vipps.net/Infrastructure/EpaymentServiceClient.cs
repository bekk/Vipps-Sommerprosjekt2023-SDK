using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vipps.net.Helpers;
using Vipps.net.Services;

namespace Vipps.net.Infrastructure
{
    public sealed class EpaymentServiceClient : BaseServiceClient
    {
        private readonly IVippsConfigurationProvider _vippsConfigurationProvider;
        private readonly AccessTokenService _accessTokenService; 

        public EpaymentServiceClient(IVippsHttpClient vippsHttpClient,
            IVippsConfigurationProvider vippsConfigurationProvider, AccessTokenService accessTokenService)
            : base(vippsHttpClient)
        {
            _vippsConfigurationProvider = vippsConfigurationProvider;
            _accessTokenService = accessTokenService; 
        }

        protected override async Task<Dictionary<string, string>> GetHeaders(
            CancellationToken cancellationToken
        )
        {
            var authToken = await _accessTokenService.GetAccessToken(cancellationToken);
            var headers = new Dictionary<string, string>
            {
                {
                    Constants.HeaderNameAuthorization,
                    $"{Constants.AuthorizationSchemeNameBearer} {authToken.Token}"
                },
                { "Idempotency-Key", Guid.NewGuid().ToString() },
                { Constants.SubscriptionKey, _vippsConfigurationProvider.GetConfiguration().SubscriptionKey }
            };
            return headers;
        }
    }
}
