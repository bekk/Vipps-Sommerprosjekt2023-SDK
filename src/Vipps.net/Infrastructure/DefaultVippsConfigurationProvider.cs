namespace Vipps.net.Infrastructure
{

    public class DefaultVippsConfigurationProvider : IVippsConfigurationProvider
    {

        private readonly VippsConfigurationOptions _vippsConfigurationOptions;

        public DefaultVippsConfigurationProvider(VippsConfigurationOptions options)
        {
            _vippsConfigurationOptions = options;
        }

        public VippsConfigurationOptions GetConfiguration()
        {
            return this._vippsConfigurationOptions;
        }
    }
}
