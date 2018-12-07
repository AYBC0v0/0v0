using Newbe.Mahua.MahuaEvents;
using System;
using System.Text.RegularExpressions;

namespace Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents
{
    public static class Common1
    {
        public static int RandMin = 90;
        public static int i = 0;
        public static int p = 100;
    }

    public class PrivateMessageFromGroupReceivedMahuaEvent
        : IPrivateMessageFromGroupReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public PrivateMessageFromGroupReceivedMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void zero(object source, System.Timers.ElapsedEventArgs e)//清空计数器
        {
            Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents.Common1.i = 0;
        }

        System.Timers.Timer timer1 = new System.Timers.Timer(3600000);//定义计时器，单位毫秒

        public void ProcessGroupMessage(PrivateMessageFromGroupReceivedContext context)
        {
            _mahuaApi.SendPrivateMessage(context.FromQq)
                .Text("输入解除口球来随机解除口球，解除概率为")
                .Text((100-Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents.Common1.RandMin).ToString())
                .Text("%")
                .Done();

            if (Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents.Common1.i<= Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents.Common1.p)
            {
                if (context.Message == "解除口球")
                {
                    Random ran = new Random();
                    int RandKey = ran.Next(0, 100);
                    if (RandKey >= Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents.Common1.RandMin)
                    {
                        _mahuaApi.RemoveBanGroupMember("675236681", context.FromQq);
                        _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("解除成功")
                        .Done();
                    }
                    Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents.Common1.i++;
                }
            }
            else
            {
                _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text("本时段全群共享解禁尝试次数已达上限，请一小时后再试///机器人会自动屏蔽故意浪费次数的人，被屏蔽后将无法使用本功能解除口球，后果自负！///")
                    .Done();
            }
            timer1.Enabled = true;
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(zero);
            timer1.AutoReset = true;
        }
    }
}
