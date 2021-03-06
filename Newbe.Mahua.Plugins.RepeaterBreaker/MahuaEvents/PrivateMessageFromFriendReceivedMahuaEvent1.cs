﻿using Newbe.Mahua.MahuaEvents;
using System;
using System.Text.RegularExpressions;
using Newbe.Mahua.Plugins.RepeaterBreaker.Services;

namespace Newbe.Mahua.Plugins.RepeaterBreaker.MahuaEvents
{
    /// <summary>
    /// 来自好友的私聊消息接收事件
    /// </summary>
    public class PrivateMessageFromFriendReceivedMahuaEvent1
        : IPrivateMessageFromFriendReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private readonly ILivegirl _livegirl;

        //getDigit()函数从字符串中截取出数字并转换成整型
        public int getDigit(string str)
        {
            Regex r = new Regex("\\d+\\.?\\d*");    //设置匹配用的正则表达式
            bool isMatch = r.IsMatch(str);
            MatchCollection mc = r.Matches(str);
            string result = string.Empty;   

            //生成处理后的字符串，应该只有数字部分
            for(int i=0; i<mc.Count; i++)
            {
                result += mc[i];
            }
            
            return int.Parse(result);   //数字字符串转换为整型数
        }

        public PrivateMessageFromFriendReceivedMahuaEvent1(
            ILivegirl livegirl,
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
            _livegirl = livegirl;
        }

        public void ProcessFriendMessage(PrivateMessageFromFriendReceivedContext context)
        {
            // todo 填充处理逻辑
            //throw new NotImplementedException();

            //功能介绍
            _mahuaApi.SendPrivateMessage(context.FromQq)
                .Text("通过和Repeater Breaker好友聊天，可以设置Repeater Breaker的运行模式")
                .Newline()
                .Text("使用以下命令，可以更改相应的运行特性")
                .Newline()
                .Text("设置复读检测次数 repeatTime=X X是3-10内的整数，是触发禁言所需的复读次数")
                .Newline()
                .Text("设置禁言时间 banTime=X X代表禁言时间，单位分钟")
                .Newline()
                .Text("调整执行模式 modeSet=X X取0、1、2，分别代表只禁言最后一个复读机，随机禁言一个复读机，禁言所有的复读机")
                .Newline()
                .Text("RandMax=X 禁言随机阈值X")
                .Newline()
                .Text("RandMin=X 解禁随机阈值X")
                .Newline()
                .Text("i=X p=X 分别设置计数器和每小时解禁上限")
                .Done();

            //匹配命令类型的正则表达式
            Regex counter = new Regex("repeatTime");
            Regex ban = new Regex("banTime");
            Regex mode = new Regex("modeSet");
            Regex RandMax = new Regex("RandMax");
            Regex RandMin = new Regex("RandMin");
            Regex p = new Regex("p");
            Regex i = new Regex("i");

            //根据不同的输入命令调整相应的参数值
            if (counter.IsMatch(context.Message)==true)
            {
                int result = getDigit(context.Message);
                Common.repeatExecuate = result;
                _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text("复读次数检测阈值已调整为 ")
                    .Text(Convert.ToString(result))
                    .Done();
            }
            
            if (ban.IsMatch(context.Message)==true)
            {
                int result = getDigit(context.Message);
                Common.execuateTime = result;
                _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text("禁言时间已调整为 ")
                    .Text(Convert.ToString(result))
                    .Done();
            }

            if (mode.IsMatch(context.Message)==true)
            {
                int result = getDigit(context.Message);
                if(result<0||result>2)
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq)
                        .Text("设定执行模式错误：模式值只能为0，1，2中的一个！")
                        .Done();
                }
                else
                {
                    Common.execuateMode = result;
                    switch(result)
                    {
                        case 0: _mahuaApi.SendPrivateMessage(context.FromQq)
                                    .Text("执行模式已设置为 常规模式，只禁言最后一个复读机")
                                    .Done();break;
                        case 1: _mahuaApi.SendPrivateMessage(context.FromQq)
                                    .Text("执行模式已设置为 随机模式，随机禁言一个复读机")
                                    .Done();break;
                        case 2: _mahuaApi.SendPrivateMessage(context.FromQq)
                                    .Text("执行模式已设置为 强力模式，禁言全部复读机")
                                    .Done();break;
                    }
                }
            }

            if (RandMax.IsMatch(context.Message)==true)
            {
                int result = getDigit(context.Message);
                Common.RandMax = result;
                _mahuaApi.SendPrivateMessage(context.FromQq)
                    .Text("随机禁言概率已调整为")
                    .Text(Convert.ToString(result))
                    .Done();
            }

            if (RandMin.IsMatch(context.Message)==true)
            {
                int result = getDigit(context.Message);
                Common1.RandMin = result;
                _mahuaApi.SendPrivateMessage(context.FromQq)
                   .Text("解禁概率已调整为")
                   .Text(Convert.ToString(result))
                   .Done();
            }

            if (p.IsMatch(context.Message)==true)
            {
                int result = getDigit(context.Message);
                Common1.p = result;
                _mahuaApi.SendPrivateMessage(context.FromQq)
                   .Text("解禁次数上限已调整为")
                   .Text(Convert.ToString(result))
                   .Done();
            }

            if (i.IsMatch(context.Message) == true)
            {
                int result = getDigit(context.Message);
                Common1.i = result;
                _mahuaApi.SendPrivateMessage(context.FromQq)
                   .Text("计数器已调整为")
                   .Text(Convert.ToString(result))
                   .Done();
            }
            if (context.FromQq == "384323693")
            {
                switch (context.Message)
                {
                    case "直播姬起飞":
                        _livegirl.StartAsync().GetAwaiter().GetResult();
                        break;
                    case "直播姬降落":
                        _livegirl.StopAsnyc().GetAwaiter().GetResult();
                        break;
                }
            }
            // 不要忘记在MahuaModule中注册
        }
    }
}
