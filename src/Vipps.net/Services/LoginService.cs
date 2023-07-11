using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Vipps.net.Infrastructure;
using Vipps.net.Models.Login;

namespace Vipps.net.Services
{

    public class LoginService
    {
        public static string GetStartLoginUri(StartLoginURIRequest startLoginUriRequest, CancellationToken cancellationToken = default)
        {
            string startLoginUri = $"{VippsConfiguration.BaseUrl}/access-management-1.0/access/oauth2/auth" +
                                   $"?client_id={VippsConfiguration.ClientId}" +
                                   $"&response_type=code" +
                                   $"&scope={startLoginUriRequest.Scope}" +
                                   $"&state={Guid.NewGuid().ToString()}" +
                                   $"&redirect_uri={startLoginUriRequest.RedirectURI}";
            
            if (startLoginUriRequest.AuthenticationMethod == AuthenticationMethod.Post)
            {
                startLoginUri = $"{startLoginUri}&response_mode=form_post";
            }

            return startLoginUri; 
        }

        public static async Task<OauthTokenResponse> GetToken(TokenRequest getTokenRequest, AuthenticationMethod authenticationMethod,
            CancellationToken cancellationToken = default)
        {
            var requestPath = "access-management-1.0/access/oauth2/token";
            if (authenticationMethod == AuthenticationMethod.Post)
            {
                getTokenRequest.Client_id = VippsConfiguration.ClientId;
                getTokenRequest.Client_secret = VippsConfiguration.ClientSecret;
                return await VippsServices.LoginServiceClientPost.ExecuteFormRequest<TokenRequest, OauthTokenResponse>(
                    requestPath,
                    HttpMethod.Post,
                    getTokenRequest,
                    cancellationToken
                );  
            }
            return await VippsServices.LoginServiceClientBasic.ExecuteFormRequest<TokenRequest, OauthTokenResponse>(
                requestPath,
                HttpMethod.Post,
                getTokenRequest,
                cancellationToken
            );
        }
    }
}
