using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Vipps.net.Infrastructure;
using Vipps.net.Models.Webhooks;

namespace Vipps.net.Services
{
    public interface IVippsWebooksService
    {
        Task<RegisterResponse> CreateWebhook(RegisterRequest registerRequest,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Webhook>> GetWebhook(CancellationToken cancellationToken = default);

        Task DeleteWebhook(string webhookId, CancellationToken cancellationToken = default);
    }


    internal sealed class VippsWebhooksService : IVippsWebooksService
    {
        private readonly WebhooksServiceClient _webhooksServiceClient;

        private readonly string requestPath = "webhooks/v1/webhooks";
        
        
        public VippsWebhooksService(WebhooksServiceClient webhooksServiceClient)
        {
            _webhooksServiceClient = webhooksServiceClient;
        }


        public async Task<RegisterResponse> CreateWebhook(RegisterRequest registerRequest,
            CancellationToken cancellationToken = default)
        {
            return await _webhooksServiceClient.ExecuteRequest<
                RegisterRequest, 
                RegisterResponse
            >(requestPath, HttpMethod.Post, registerRequest, cancellationToken);
        }

        public async Task<IEnumerable<Webhook>> GetWebhook(CancellationToken cancellationToken = default)
        {
            var response = await _webhooksServiceClient.ExecuteRequest<QueryResponse>(requestPath, HttpMethod.Get, cancellationToken);
            return response.Webhooks;
        }

        public async Task DeleteWebhook(string webhookId, CancellationToken cancellationToken = default)
        {
            await _webhooksServiceClient.ExecuteRequest($"{requestPath}/{webhookId}" , HttpMethod.Delete, new StringContent(string.Empty) ,cancellationToken);
        }
    }
}
