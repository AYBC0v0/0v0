using Newbe.Mahua.MahuaEvents;
using System;
using System.Text.RegularExpressions;

namespace Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents
{
    public static class Common1
    {
        public static int RandMin = 90;
        public static int i = 0;//计数器
        public static int p = 15;//计数器上限
        public static string a = "0";//记录QQ号
        public static System.Collections.Generic.List<string> List1 = new System.Collections.Generic.List<string>();
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

        public void zero(object source, System.Timers.ElapsedEventArgs e)//恢复计数器，每次恢复一点
        {
            if(Common1.i>0)
            {
                Common1.i--;
            }
        }

        System.Timers.Timer timer1 = new System.Timers.Timer(300000);//定义计时器，单位毫秒

        public void ProcessGroupMessage(PrivateMessageFromGroupReceivedContext context)
        {
            _mahuaApi.SendPrivateMessage(context.FromQq)
                .Text("输入解除口球来随机解除口球，解除概率为")
                .Text((100- Common1.RandMin).ToString())
                .Text("%")
                .Done();

            if (Common1.i<= Common1.p)
            {
                if (context.Message == "解除口球")
                {
                    if (context.FromQq == Common1.a)
                    {
                        Common1.i++;
                    }
                    Common1.a = context.FromQq;
                    Random ran = new Random();
                    int RandKey = ran.Next(0, 100);
                    if (RandKey >= Common1.RandMin)
                    {
                        _mahuaApi.RemoveBanGroupMember("675236681", context.FromQq);
                        _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("解除成功")
                        .Done();
                    }
                }
            }
            else
            {
                _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text("本时段解禁尝试次数已达上限，请等待计数器自动回复后再试（每5分钟自动回复1点）")
                    .Done();
            }
            timer1.Enabled = true;
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(zero);
            timer1.AutoReset = true;
        }
    }
}
