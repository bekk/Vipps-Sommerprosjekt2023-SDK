using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Polly.Retry;
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

        public static async Task<OauthTokenResponse> GetWebLoginToken(TokenRequest getTokenRequest, AuthenticationMethod authenticationMethod,
            CancellationToken cancellationToken = default)
        {
            var requestPath = "access-management-1.0/access/oauth2/token";
            getTokenRequest.Grant_type = "authorization_code";
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

        public static async Task<InitCibaResponse> InitCiba(InitCibaRequest initCibaRequest,
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
                initCibaBody.ClientId = VippsConfiguration.ClientId;
                initCibaBody.ClientSecret = VippsConfiguration.ClientSecret;
                return await VippsServices.LoginServiceClientPost.ExecuteFormRequest<InitCibaBody, InitCibaResponse>(
                    requestPath,
                    HttpMethod.Post,
                    initCibaBody,
                    cancellationToken
                );  
            }
            return await VippsServices.LoginServiceClientBasic.ExecuteFormRequest<InitCibaBody, InitCibaResponse>(
                requestPath,
                HttpMethod.Post,
                initCibaBody,
                cancellationToken
            );
        }

        public static async Task<OauthTokenResponse> GetCibaTokenNoRedirect(string authReqId, 
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
                cibaTokenRequest.ClientId = VippsConfiguration.ClientId;
                cibaTokenRequest.ClientSecret = VippsConfiguration.ClientSecret;
                return await VippsServices.LoginServiceClientPost.ExecuteFormRequest<CibaTokenNoRedirectRequest, OauthTokenResponse>(
                    requestPath,
                    HttpMethod.Post,
                    cibaTokenRequest,
                    cancellationToken
                );
            }
            return await VippsServices.LoginServiceClientBasic.ExecuteFormRequest<CibaTokenNoRedirectRequest, OauthTokenResponse>(
                requestPath,
                HttpMethod.Post,
                cibaTokenRequest,
                cancellationToken
            );
        }

        public static async Task<OauthTokenResponse> GetCibaTokenRedirect(string code,
            AuthenticationMethod authenticationMethod, CancellationToken cancellationToken = default)
        {
            var cibaTokenRequest = new CibaTokenRedirectRequest
            {
                Code = code, GrantType = "urn:vipps:params:grant-type:ciba-redirect"
            };
            var requestPath = "access-management-1.0/access/oauth2/token";
            
            if (authenticationMethod == AuthenticationMethod.Post)
            {
                cibaTokenRequest.ClientId = VippsConfiguration.ClientId;
                cibaTokenRequest.ClientSecret = VippsConfiguration.ClientSecret;
                return await VippsServices.LoginServiceClientPost.ExecuteFormRequest<CibaTokenRedirectRequest, OauthTokenResponse>(
                    requestPath,
                    HttpMethod.Post,
                    cibaTokenRequest,
                    cancellationToken
                );
            }
            return await VippsServices.LoginServiceClientBasic.ExecuteFormRequest<CibaTokenRedirectRequest, OauthTokenResponse>(
                requestPath,
                HttpMethod.Post,
                cibaTokenRequest,
                cancellationToken
            );
        }
    }
}
