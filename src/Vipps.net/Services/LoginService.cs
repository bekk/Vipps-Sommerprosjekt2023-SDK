using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Vipps.net.Infrastructure;
using Vipps.net.Models.Login;

namespace Vipps.net.Services
{
    public interface IVippsLoginService
    {
        string GetStartLoginUri(StartLoginURIRequest startLoginUriRequest, CancellationToken cancellationToken = default);

        Task<OauthTokenResponse> GetWebLoginToken(TokenRequest getTokenRequest, AuthenticationMethod authenticationMethod,
            CancellationToken cancellationToken = default);

        Task<InitCibaResponse> InitCiba(InitCibaRequest initCibaRequest,
            AuthenticationMethod authenticationMethod, CancellationToken cancellationToken = default);

        Task<OauthTokenResponse> GetCibaTokenNoRedirect(string authReqId, 
            AuthenticationMethod authenticationMethod, CancellationToken cancellationToken = default);

        Task<OauthTokenResponse> GetCibaTokenRedirect(string code,
            AuthenticationMethod authenticationMethod, CancellationToken cancellationToken = default);
    }

    public class LoginService : IVippsLoginService
    {
        private readonly IVippsConfigurationProvider _vippsConfigurationProvider;
        private readonly LoginServiceClientBasic _loginServiceClientBasic;
        private readonly LoginServiceClientPost _loginServiceClientPost; 
        public LoginService(IVippsConfigurationProvider vippsConfigurationProvider, LoginServiceClientBasic loginServiceClientBasic, LoginServiceClientPost loginServiceClientPost)
        {
            _vippsConfigurationProvider = vippsConfigurationProvider;
            _loginServiceClientBasic = loginServiceClientBasic;
            _loginServiceClientPost = loginServiceClientPost;
        }
        
        public string GetStartLoginUri(StartLoginURIRequest startLoginUriRequest, CancellationToken cancellationToken = default)
        {
            var options = _vippsConfigurationProvider.GetConfiguration(); 
            var baseUrl = options.UseTestMode 
                ? "https://apitest.vipps.no" 
                : "https://api.vipps.no";
            
            string startLoginUri = $"{baseUrl}/access-management-1.0/access/oauth2/auth" +
                                   $"?client_id={options.ClientId}" +
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

        public async Task<OauthTokenResponse> GetWebLoginToken(TokenRequest getTokenRequest, AuthenticationMethod authenticationMethod,
            CancellationToken cancellationToken = default)
        {
            var options = _vippsConfigurationProvider.GetConfiguration();  
            var requestPath = "access-management-1.0/access/oauth2/token";
            getTokenRequest.Grant_type = "authorization_code";
            if (authenticationMethod == AuthenticationMethod.Post)
            {
                getTokenRequest.Client_id = options.ClientId;
                getTokenRequest.Client_secret = options.ClientSecret;
                return await _loginServiceClientPost.ExecuteFormRequest<TokenRequest, OauthTokenResponse>(
                    requestPath,
                    HttpMethod.Post,
                    getTokenRequest,
                    cancellationToken
                );  
            }
            return await _loginServiceClientBasic.ExecuteFormRequest<TokenRequest, OauthTokenResponse>(
                requestPath,
                HttpMethod.Post,
                getTokenRequest,
                cancellationToken
            );
        }

        public async Task<InitCibaResponse> InitCiba(InitCibaRequest initCibaRequest,
            AuthenticationMethod authenticationMethod, CancellationToken cancellationToken = default)
        {
            var initCibaBody = new InitCibaBody
            {
                Scope = initCibaRequest.Scope,
                LoginHint = $"urn:mobilenumber:{initCibaRequest.PhoneNumber}",
                State = Guid.NewGuid().ToString(),
                BindingMessage = initCibaRequest.BindingMessage.ToUpper(),
            };

            if (initCibaRequest.RedirectUri != null)
            {
                initCibaBody.RedirectUri = initCibaRequest.RedirectUri;
                initCibaBody.RequestedFlow = "login_to_webpage";
            }
            
            
            var requestPath = "vipps-login-ciba/api/backchannel/authentication";

            if (authenticationMethod == AuthenticationMethod.Post)
            {
                var options = _vippsConfigurationProvider.GetConfiguration(); 
                initCibaBody.ClientId = options.ClientId;
                initCibaBody.ClientSecret = options.ClientSecret;
                return await _loginServiceClientPost.ExecuteFormRequest<InitCibaBody, InitCibaResponse>(
                    requestPath,
                    HttpMethod.Post,
                    initCibaBody,
                    cancellationToken
                );  
            }
            return await _loginServiceClientBasic.ExecuteFormRequest<InitCibaBody, InitCibaResponse>(
                requestPath,
                HttpMethod.Post,
                initCibaBody,
                cancellationToken
            );
        }

        public async Task<OauthTokenResponse> GetCibaTokenNoRedirect(string authReqId, 
            AuthenticationMethod authenticationMethod, CancellationToken cancellationToken = default)
        {
            var cibaTokenRequest = new CibaTokenNoRedirectRequest
            {
                AuthReqId = authReqId,
                GrantType = "urn:openid:params:grant-type:ciba",
            };
            var requestPath = "access-management-1.0/access/oauth2/token";

            if (authenticationMethod == AuthenticationMethod.Post)
            {
                var options = _vippsConfigurationProvider.GetConfiguration(); 
                cibaTokenRequest.ClientId = options.ClientId;
                cibaTokenRequest.ClientSecret = options.ClientSecret;
                return await _loginServiceClientPost.ExecuteFormRequest<CibaTokenNoRedirectRequest, OauthTokenResponse>(
                    requestPath,
                    HttpMethod.Post,
                    cibaTokenRequest,
                    cancellationToken
                );
            }
            return await _loginServiceClientBasic.ExecuteFormRequest<CibaTokenNoRedirectRequest, OauthTokenResponse>(
                requestPath,
                HttpMethod.Post,
                cibaTokenRequest,
                cancellationToken
            );
        }

        public async Task<OauthTokenResponse> GetCibaTokenRedirect(string code,
            AuthenticationMethod authenticationMethod, CancellationToken cancellationToken = default)
        {
            var cibaTokenRequest = new CibaTokenRedirectRequest
            {
                Code = code, GrantType = "urn:vipps:params:grant-type:ciba-redirect"
            };
            var requestPath = "access-management-1.0/access/oauth2/token";
            
            if (authenticationMethod == AuthenticationMethod.Post)
            {
                var options = _vippsConfigurationProvider.GetConfiguration(); 
                cibaTokenRequest.ClientId = options.ClientId;
                cibaTokenRequest.ClientSecret = options.ClientSecret;
                return await _loginServiceClientPost.ExecuteFormRequest<CibaTokenRedirectRequest, OauthTokenResponse>(
                    requestPath,
                    HttpMethod.Post,
                    cibaTokenRequest,
                    cancellationToken
                );
            }
            return await _loginServiceClientBasic.ExecuteFormRequest<CibaTokenRedirectRequest, OauthTokenResponse>(
                requestPath,
                HttpMethod.Post,
                cibaTokenRequest,
                cancellationToken
            );
        }
    }
}
