using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Plugins.RepeaterBreaker.Services;

namespace Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents
{
    /// <summary>
    /// 插件初始化事件
    /// </summary>
    public class InitializationMahuaEvent
        : IInitializationMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private readonly IWebHost _webHost;

        public InitializationMahuaEvent(
            IMahuaApi mahuaApi,
            IWebHost webHost)
        {
            _mahuaApi = mahuaApi;
            _webHost = webHost;
        }

        public void Initialized(InitializedContext context)
        {
            // 在本地地址上启动Web服务，可以根据需求改变端口
            _webHost.StartAsync("http://localhost:65238", _mahuaApi.GetSourceContainer());
        }
    }
}
