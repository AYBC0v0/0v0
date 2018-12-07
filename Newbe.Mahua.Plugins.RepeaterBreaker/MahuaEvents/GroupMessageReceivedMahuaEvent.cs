using Newbe.Mahua.MahuaEvents;
using System;
using System.Linq;

namespace Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents
{
    public static class Common
    {
        //存储全局静态变量
        public static int repeatTime = 0;   //复读次数，初值为0
        public static string msg = "000";   //截获群消息用于复读行为检测
        public static int repeatExecuate = 4;   //检测复读次数，可通过私戳消息自定义
        public static System.Collections.Generic.List<string> repeaterList = new System.Collections.Generic.List<string>();   //参与复读的复读机名单
        public static int execuateTime = 3;   //禁言时间
        public static int execuateMode = 1;   //执行模式
        public static double RandMax = 0.5;   //禁言阈值
        public static int a = 1;   //开关
    }
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMessageReceivedMahuaEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupMessageReceivedMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            
            //throw new NotImplementedException();

            //复读计数器计数，连续两个消息内容相同，计数器+1并将发言的群成员加入复读机成员名单，反之重置计数器和名单
            if (Common.msg == context.Message)
            {
                Common.repeatTime++;
                Common.repeaterList.Add(context.FromQq);
            }
            else
            {
                Common.msg = context.Message;
                Common.repeaterList.Clear();
                Common.repeatTime = 0;
            }

            //复读事件触发，根据执行模式进行禁言操作
            if (Common.repeatTime == Common.repeatExecuate)
            {
                //正常禁言：禁言最后一个发言的复读机
                if (Common.execuateMode==0)
                {
                    _mahuaApi.BanGroupMember(context.FromGroup, context.FromQq, TimeSpan.FromMinutes(Common.execuateTime));
                    _mahuaApi.SendGroupMessage(context.FromGroup)
                        .Text("复读机")
                        .At(context.FromQq)
                        .Text("已被当场逮捕")
                        .Done();
                }

                //随机禁言：从复读机名单中随机选择一个禁言
                if (Common.execuateMode == 1)
                {
                    Random ran = new Random();
                    int RandKey = ran.Next(0, Common.repeaterList.Count()-1);
                    _mahuaApi.BanGroupMember(context.FromGroup, Common.repeaterList[RandKey], TimeSpan.FromMinutes(Common.execuateTime));
                    _mahuaApi.SendGroupMessage(context.FromGroup)
                        .Text("复读机")
                        .At(Common.repeaterList[RandKey])
                        .Text("已被当场逮捕")
                        .Done();

                }

                //强力禁言：禁言在复读机名单中的额所有人
                if (Common.execuateMode==2)
                {
                    for(int k=0; k<= Common.repeaterList.Count; k++)
                    {
                        _mahuaApi.BanGroupMember(context.FromGroup, Common.repeaterList[k], TimeSpan.FromMinutes(Common.execuateTime));
                    }
                    _mahuaApi.SendGroupMessage(context.FromGroup)
                      .Text("所有复读机均已被逮捕")
                      .Newline()
                      .Text("///Powerful Mode Enabled///强力模式启用中///Powerful Mode Enabled///")
                      .Done();
                }

                //重置计数器和复读机名单
                Common.repeatTime = 0;
                Common.repeaterList.Clear();
            }
            //复读模块
            Random ran1 = new Random();
            double RandKey1 = ran1.Next(0, 100);
            if(RandKey1 <= Common.RandMax)
            {
                _mahuaApi.SendGroupMessage(context.FromGroup)
                    .Text(context.Message)
                    .Done();
            }
            //全随机模块
            Random ran2 = new Random();
            double RandKey2 = ran2.Next(0, 100);
            if(RandKey2 <= Common.RandMax)
            {
                _mahuaApi.BanGroupMember(context.FromGroup, context.FromQq, TimeSpan.FromMinutes(Common.execuateTime));
                _mahuaApi.SendGroupMessage(context.FromGroup)
                    .At(context.FromQq)
                    .Text("已被丢入小黑屋并穿上小裙子")
                    .Done();
            }
            //口球抽奖模块
            if(Common.msg =="口球抽奖")
            {
                Random ran3 = new Random();
                int RandKey3 = ran3.Next(0, 43200);
                _mahuaApi.BanGroupMember(context.FromGroup, context.FromQq, TimeSpan.FromMinutes(RandKey3));
                _mahuaApi.SendGroupMessage(context.FromGroup)
                    .Text("恭喜")
                    .At(context.FromQq)
                    .Text("中奖！口球时间为")
                    .Text(RandKey3.ToString())
                    .Text("分钟")
                    .Done();
                
            }
            // 不要忘记在MahuaModule中注册
            }
    }
}
