using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Vipps.net.Infrastructure;
using Vipps.net.Models.AccessToken;

namespace Vipps.net.Services
{
    public interface IVippsAccessTokenService
    {
        Task<AccessToken> GetAccessToken(
            CancellationToken cancellationToken = default
        );
    }

    public class AccessTokenService : IVippsAccessTokenService
    {

        private readonly IVippsConfigurationProvider _vippsConfigurationProvider;
        private readonly AccessTokenServiceClient _accessTokenServiceClient;
        private readonly AccessTokenCacheService _accessTokenCacheService; 
        public AccessTokenService(IVippsConfigurationProvider vippsConfigurationProvider, AccessTokenServiceClient accessTokenServiceClient, AccessTokenCacheService accessTokenCacheService)
        {
            _vippsConfigurationProvider = vippsConfigurationProvider;
            _accessTokenServiceClient = accessTokenServiceClient;
            _accessTokenCacheService = accessTokenCacheService; 
        }
        
        public async Task<AccessToken> GetAccessToken(
            CancellationToken cancellationToken = default
        )
        {
            var options = _vippsConfigurationProvider.GetConfiguration(); 
            var key = $"{options.ClientId}{options.ClientSecret}";
            var cachedToken = _accessTokenCacheService.Get(key);
            if (cachedToken != null)
            {
                return cachedToken;
            }

            var accessToken =
                await _accessTokenServiceClient.ExecuteRequest<AccessToken>(
                    "/accesstoken/get",
                    HttpMethod.Post,
                    cancellationToken
                );
            _accessTokenCacheService.Add(key, accessToken);
            return accessToken;
        }
    }
}
