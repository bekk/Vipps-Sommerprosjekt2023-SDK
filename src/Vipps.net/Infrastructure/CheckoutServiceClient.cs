using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vipps.net.Helpers;

namespace Vipps.net.Infrastructure
{
    public sealed class CheckoutServiceClient : BaseServiceClient
    {
        private readonly IVippsConfigurationProvider _vippsConfigurationProvider;

        internal CheckoutServiceClient(IVippsHttpClient vippsHttpClient,
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
                    { Constants.HeaderNameClientId, options.ClientId },
                    { Constants.HeaderNameClientSecret, options.ClientSecret },
                    { Constants.SubscriptionKey, options.SubscriptionKey },
                }
            );
        }
    }
}
