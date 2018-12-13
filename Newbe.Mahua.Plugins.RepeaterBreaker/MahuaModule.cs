using Autofac;
using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents;
using Newbe.Mahua.Plugins.RepeaterBreaker.Services;
using Newbe.Mahua.Plugins.RepeaterBreaker.Services.Impl;

namespace Newbe.Mahua.Plugins.RepeaterBreaker
{
    /// <summary>
    /// Ioc容器注册
    /// </summary>
    public class MahuaModule : IMahuaModule
    {
        public Module[] GetModules()
        {
            // 可以按照功能模块进行划分，此处可以改造为基于文件配置进行构造。实现模块化编程。
            return new Module[]
            {
                new PluginModule(),
                new MahuaEventsModule(),
                new MyServiceModule(),
            };
        }

        /// <summary>
        /// 基本模块
        /// </summary>
        private class PluginModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                // 将实现类与接口的关系注入到Autofac的Ioc容器中。如果此处缺少注册将无法启动插件。
                // 注意！！！PluginInfo是插件运行必须注册的，其他内容则不是必要的！！！
                builder.RegisterType<PluginInfo>()
                    .As<IPluginInfo>();

                //注册在“设置中心”中注册菜单，若想订阅菜单点击事件，可以查看教程。http://www.newbe.pro/docs/mahua/2017/12/24/Newbe-Mahua-Navigations.html
                builder.RegisterType<MyMenuProvider>()
                    .As<IMahuaMenuProvider>();
            }
        }

        /// <summary>
        /// <see cref="IMahuaEvent"/> 事件处理模块
        /// </summary>
        private class MahuaEventsModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                builder.RegisterType<MahuaEvents.GroupMessageReceivedMahuaEvent>()
                    .As<Mahua.MahuaEvents.IGroupMessageReceivedMahuaEvent>();
                builder.RegisterType<MahuaEvents.PrivateMessageFromFriendReceivedMahuaEvent1>()
                    .As<Mahua.MahuaEvents.IPrivateMessageFromFriendReceivedMahuaEvent>();
                builder.RegisterType<MahuaEvents.PrivateMessageFromGroupReceivedMahuaEvent>()
                    .As<Mahua.MahuaEvents.IPrivateMessageFromGroupReceivedMahuaEvent>();
                builder.RegisterType<MahuaEvents.InitializationMahuaEvent>()
                    .As<Mahua.MahuaEvents.IInitializationMahuaEvent>();
                // 将需要监听的事件注册，若缺少此注册，则不会调用相关的实现类
            }
        }
        private class MyServiceModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                // 确保Web服务是单例
                builder.RegisterType<OwinWebHost>()
                    .As<IWebHost>()
                    .SingleInstance();

                // AsSelf是为了Hangfire能够初始化这个类
                builder.RegisterType<Livegirl>()
                    .As<ILivegirl>()
                    .AsSelf();
                builder.RegisterType<LiveRoom>()
                    .As<ILiveRoom>();
            }
        }
    }
}
