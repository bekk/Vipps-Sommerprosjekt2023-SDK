using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vipps.net.Helpers;
using Vipps.net.Models.Login;

namespace Vipps.net.Infrastructure
{
    internal sealed class LoginServiceClientPost : BaseServiceClient
    {
        internal LoginServiceClientPost(IVippsHttpClient vippsHttpClient)
            : base(vippsHttpClient) { }

        protected override async Task<Dictionary<string, string>> GetHeaders(
            CancellationToken cancellationToken 
        )  
        {
            return await Task.FromResult(
                new Dictionary<string, string>
                {
                }
            );
        }
    }
}
