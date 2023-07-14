namespace Vipps.net.Infrastructure
{
    public interface IVippsConfigurationProvider
    {
        VippsConfigurationOptions GetConfiguration();
    }
}
