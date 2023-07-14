using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Vipps.net.Infrastructure;
using Vipps.net.Services;

namespace Vipps.net
{
    public interface IVippsApi
    {
        IVippsAccessTokenService AccessTokenService();
        IVippsEpaymentService EpaymentService();
        IVippsLoginService LoginService();
        IVippsCheckoutService CheckoutService();
    }

    public class VippsApi : IVippsApi
    {
        private readonly IVippsConfigurationProvider configurationProvider;
        private VippsHttpClient _vippsHttpClient;
        private ILoggerFactory _loggerFactory;
        private readonly AccessTokenService _accessTokenService;

        public static IVippsApi Create(IVippsConfigurationProvider configurationProvider,
            ILoggerFactory loggerFactory = null)
        {
            return new VippsApi(configurationProvider, loggerFactory);
        }

        public static IVippsApi Create(VippsConfigurationOptions options, ILoggerFactory loggerFactory = null)
        {
            return new VippsApi(new DefaultVippsConfigurationProvider(options), loggerFactory);
        }

        private VippsApi(IVippsConfigurationProvider configurationProvider, ILoggerFactory loggerFactory = null)
        {
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            this.configurationProvider = configurationProvider;
            _vippsHttpClient = new VippsHttpClient(new HttpClient(), configurationProvider.GetConfiguration());

            _accessTokenService = new AccessTokenService(configurationProvider,
                new AccessTokenServiceClient(_vippsHttpClient, this.configurationProvider),
                new AccessTokenCacheService());
        }

        public IVippsAccessTokenService AccessTokenService()
        {
            return this._accessTokenService;
        }

        public IVippsEpaymentService EpaymentService()
        {
            return new EpaymentService(new EpaymentServiceClient(_vippsHttpClient, configurationProvider,
                _accessTokenService));
        }

        public IVippsLoginService LoginService()
        {
            return new LoginService(configurationProvider
                , new LoginServiceClientBasic(_vippsHttpClient, configurationProvider),
                new LoginServiceClientPost(_vippsHttpClient));
        }

        public IVippsCheckoutService CheckoutService()
        {
            return new CheckoutService(new CheckoutServiceClient(_vippsHttpClient, configurationProvider));
        }
    }
}
