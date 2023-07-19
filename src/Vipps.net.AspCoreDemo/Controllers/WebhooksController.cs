using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vipps.net.Models.Webhooks;
using Vipps.net.Services;

namespace Vipps.net.AspCore31Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhooksController : ControllerBase
    {
        private readonly IVippsWebooksService _vippsWebooksService;

        public WebhooksController(IVippsWebooksService vippsWebooksService)
        {
            _vippsWebooksService = vippsWebooksService;
        }


        [HttpPost("Create")]
        public async Task<RegisterResponse> Create()
        {
            RegisterRequest registerRequest = new RegisterRequest()
            {
                Url = new Uri("https://webhook.site/ebcd8e27-5031-4dd5-beb7-a2d491cd7641"),
                Events = new List<string>
                {
                    WebhookEvent.EPaymentCreated
                }
            };

            return await _vippsWebooksService.CreateWebhook(registerRequest);
        }
        
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            var response = await _vippsWebooksService.GetWebhook();
            return new OkObjectResult(response);
        }

        [HttpDelete("Delete{webhookId}")]
        public async Task Delete(string webhookId)
        {
            await _vippsWebooksService.DeleteWebhook(webhookId);
        }
    }
}
