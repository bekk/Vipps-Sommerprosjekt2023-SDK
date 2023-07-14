﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vipps.net.Models.Login;
using Vipps.net.Services;

namespace Vipps.net.AspCore31Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<CheckoutController> _logger;
        private readonly IVippsLoginService _loginService; 

        public LoginController(ILogger<CheckoutController> logger, IVippsLoginService loginService)
        {
            _logger = logger;
            _loginService = loginService; 
        }

        [HttpGet]
        public string GetStartLoginURI()
        {
            StartLoginURIRequest startLoginUriRequest = new StartLoginURIRequest()
            {
                RedirectURI = "http://localhost:3000",
                Scope = "openid email name phoneNumber",
                AuthenticationMethod = AuthenticationMethod.Basic
            };
            
            return _loginService.GetStartLoginUri(startLoginUriRequest);
        }

        [HttpPost("/token/{code}")]
        public async Task<OauthTokenResponse> GetWebLoginToken(string code)
        {

            TokenRequest getTokenRequest = new TokenRequest
            {
                Redirect_uri = "http://localhost:3000",
                Code = code
            };
            return await _loginService.GetWebLoginToken(getTokenRequest,AuthenticationMethod.Basic);
        }

        [HttpPost("/init-ciba")]
        public async Task<InitCibaResponse> InitCiba()
        {
            InitCibaRequest initCibaRequest = new InitCibaRequest
            {
                Scope = "openid email name phoneNumber", 
                PhoneNumber = "40285760", 
                BindingMessage = "XYZ-123",
            };
            return await _loginService.InitCiba(initCibaRequest, AuthenticationMethod.Basic);
        }

        [HttpPost("/ciba-token-no-redirect{authReqId}")]
        public async Task<OauthTokenResponse> GetCibaTokenNoRedirect(string authReqId)
        {
            return await _loginService.GetCibaTokenNoRedirect(authReqId, AuthenticationMethod.Basic);
        }

        [HttpPost("/ciba-token-redirect{code}")]
        public async Task<OauthTokenResponse> GetCibaTokenRedirect(string code)
        {
            return await _loginService.GetCibaTokenRedirect(code, AuthenticationMethod.Basic);
        }
    }
}
