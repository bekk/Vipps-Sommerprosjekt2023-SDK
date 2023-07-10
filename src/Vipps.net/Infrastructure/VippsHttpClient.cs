﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Vipps.net.Infrastructure
{
    public class VippsHttpClient : IVippsHttpClient
    {
        private HttpClient _httpClient;
        private readonly TimeSpan DefaultTimeOut = TimeSpan.FromSeconds(100);

        public VippsHttpClient() { }

        public VippsHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Uri BaseAddress
        {
            get { return HttpClient.BaseAddress; }
        }

        internal HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = CreateDefaultHttpClient();
                }

                return _httpClient;
            }
        }

        public async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            
                var headers = GetHeaders();
                foreach (var header in headers)
                {
                    if (request.Headers.Contains(header.Key))
                    {
                        request.Headers.Remove(header.Key);
                    }
                
                    request.Headers.Add(header.Key, header.Value);
                }

                int i = 0;
            var response = await HttpClient
                .SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
            return response;
        }

        private HttpClient CreateDefaultHttpClient()
        {
            var httpClient = new HttpClient()
            {
                Timeout = DefaultTimeOut,
                BaseAddress = new Uri($"{VippsConfiguration.BaseUrl}")
            };

            return httpClient;
        }

        private static Dictionary<string, string> GetHeaders()
        {
            var assemblyName = typeof(VippsConfiguration).Assembly.GetName();
            return new Dictionary<string, string>
            {
                { "Merchant-Serial-Number", VippsConfiguration.MerchantSerialNumber },
                { "Vipps-System-Name", assemblyName.Name },
                { "Vipps-System-Version", assemblyName.Version.ToString() },
                { "Vipps-System-Plugin-Name", VippsConfiguration.PluginName },
                { "Vipps-System-Plugin-Version", VippsConfiguration.PluginVersion }
            };
        }
    }
}
