﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vipps.net.Helpers;
using Vipps.net.Services;

namespace Vipps.net.Infrastructure
{
    internal sealed class EpaymentServiceClient : BaseServiceClient
    {
        internal EpaymentServiceClient(IVippsHttpClient vippsHttpClient)
            : base(vippsHttpClient) { }

        protected override async Task<Dictionary<string, string>> GetHeaders(
            CancellationToken cancellationToken
        )
        {
            var authToken = await AccessTokenService.GetAccessToken(cancellationToken);
            var headers = new Dictionary<string, string>
            {
                {
                    Constants.HeaderNameAuthorization,
                    $"{Constants.AuthorizationSchemeNameBearer} {authToken.Token}"
                },
                { "Idempotency-Key", Guid.NewGuid().ToString() },
                { Constants.SubscriptionKey, VippsConfiguration.SubscriptionKey }
            };
            return headers;
        }
    }
}
