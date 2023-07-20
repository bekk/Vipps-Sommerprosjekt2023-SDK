using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Vipps.net.Models.Webhooks;


namespace Vipps.net.IntegrationTests;

[TestClass]
public class WebhooksTests
{
    [TestMethod]
    public async Task CreateGetDeleteWebhook()
    {
        IVippsApi vippsApi = TestSetup.CreateVippsAPI();
        
        RegisterRequest registerRequest = new RegisterRequest()
        {
            Url = new Uri("https://webhook.site/ebcd8e27-5031-4dd5-beb7-a2d491cd7641"),
            Events = new List<string>
            {
                WebhookEvent.EPaymentCreated
            }
        };
        
        var createWebhookResponse = await vippsApi.WebooksService().CreateWebhook(registerRequest);
        Assert.IsNotNull(createWebhookResponse.Id);
        Assert.IsNotNull(createWebhookResponse.Secret);

        var getWebHooksResponse = await vippsApi.WebooksService().GetWebhook();
        Assert.IsNotNull(getWebHooksResponse);
        var constainsCreatedWebhook = getWebHooksResponse.Any(webhook =>
            webhook.Url == registerRequest.Url &&
            webhook.Events.SequenceEqual(registerRequest.Events) &&
            webhook.Id == createWebhookResponse.Id);        
        Assert.IsTrue(constainsCreatedWebhook);
        
        
        await vippsApi.WebooksService().DeleteWebhook(createWebhookResponse.Id.ToString());
    }
}
