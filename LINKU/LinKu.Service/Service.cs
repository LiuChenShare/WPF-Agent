using LinKu.Models;
using LinKu.Models.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LinKu
{
    public class Service
    {
        /// <summary>
        /// 消息包
        /// </summary>
        static MsgBase message = new MsgBase();
        /// <summary>
        /// 回调传递任务百分比进度
        /// </summary>
        public static void MessageStep(Action<MsgBase> action, int step)
        {
            message.Value = step;
            message.State = MsgState.Step;
            action(message);
        }
        /// <summary>
        /// 登录
        /// </summary>
        public static void Login(Action<MsgBase> action)
        {
            Task.Factory.StartNew(() =>
            {
                MessageStep(action, 30);
                ///调用接口
                string ret = Http.Get("http://www.kailitec.com/api/userapi.php?" + "username=" + Auth.user.Name + "&password=" + Auth.user.Pwd);
                MessageStep(action, 60);
                Auth.InternalClass = JsonConvert.DeserializeObject<InternalClass>(ret);


                //暴力破解
                Auth.InternalClass.status = "OK";


                //创建回发消息
                MessageStep(action, 100);
                switch (Auth.InternalClass.status)
                {
                    case "OK"://登录成功
                        message.State = MsgState.Susuccess;
                        Auth.IsLogin = true;
                        //回调
                        action(message);
                        return;
                    case "Connect Error":
                        message.Message = "验证连接错误!";
                        message.State = MsgState.Error;
                        break;
                    case "Password Error":
                        message.Message = "用户密码错误!";
                        message.State = MsgState.Error;
                        break;
                    case "None":
                        message.Message = "您输入的账号不存在!";
                        message.State = MsgState.Error;
                        break;
                    case "STOP":
                        message.Message = "您的会员已经过期,请购买会员后使用!";
                        message.State = MsgState.Error;
                        break;
                }
                //回调
                action(message);
            });
        }

        /// <summary>
        /// 读取配置信息-这个方法可以不用回调，主窗体不会等待这个方法执行完成
        /// </summary>
        public static void ReadConfig(Action<MsgBase> action)
        {
            Task.Factory.StartNew(() =>
            {

          


                //1.运行程序读取程序目录config.ini 配置文件 内容格式：
                //[config]
                //username = 5870352@qq.com
                //password = 5DOL5 / fC5P8 =
                //remember = 1
                //method = 1
                //game = H1Z1亚太
                //gameFN = H1Z1
                //automode = yes
                //gameroute = yes
                //mtu = 1260
                //type = 0
                //ICS = 1

                //读取字段username为用户名，password为加密后的密码，
                //remember为是否记住密码，method为加速模式，game为上次加速的游戏名称，
                //gameFN为游戏图片名，automod为自动选择节点模式，
                //gameroute为加速方式（yes游戏加速模式，no智能加速模式），mtu值，type值，ICS值，以上的值都需要传递给主窗口。


                //****************************读取完成之后 存到 Models层的一个 Config 静态类 中
            });
        }

        /// <summary>
        /// 读取游戏节点
        /// </summary>
        public static void LoadNetworkNode(Action<MsgBase> action)
        {
            Task.Factory.StartNew(() =>
            {
                ///调用接口
                string ret = Http.Get("http://www.kailitec.com/api/serverList.php?" + "key=" + Auth.Key + "&server=" + Auth.SelectGame.Name + "&username" + Auth.user.Name);
                JObject obj = JsonConvert.DeserializeObject<JObject>(ret);
                JArray ar = (JArray)obj["data"];
                Auth.NetworkNodeList.Clear();
                foreach (JObject item in ar)
                {
                    Auth.NetworkNodeList.Add(new NetworkNode()
                    {
                        Name = item["name"].ToString(),
                        Io = item["io"].ToString(),
                        Group = item["group"].ToString(),
                        Id = item["id"].ToString(),
                        Info = item["info"].ToString(),
                        Ip = item["ip"].ToString(),
                        Status = item["status"].ToString(),
                        Tunping = Convert.ToInt32(item["tunping"]),
                        Gamename = item["gamename"].ToString()
                    });
                }
                action(new MsgBase() { State = MsgState.Susuccess });
            });
        }

        /// <summary>
        /// 读取游戏列表 跟 游戏分组
        /// </summary>
        /// <param name="action"></param>
        public static void LoadGameList(Action<MsgBase> action)
        {
            Task.Factory.StartNew(() =>
            {
                Auth.NetworkNodeList = new List<NetworkNode>();
                ///调用接口
                string ret = Http.Get("http://www.kailitec.com/api/GameListJson.php?" + "key=" + Auth.Key);
                //{"id":"0","name":"全球服","group":"HK","picname":"全球服",}
                JObject obj = JsonConvert.DeserializeObject<JObject>(ret);
                JArray ar = (JArray)obj["data"];
                foreach (JObject item in ar)
                {
                    Auth.GameList.Add(new Game()
                    {
                        Name = item["name"].ToString(),
                        Group = item["group"].ToString(),
                        Id = item["id"].ToString(),
                        Hot = (int)item["hot"],
                        Image = "http://www.kailitec.com/update/gamespic/" + item["picname"].ToString() + ".png"
                    });
                }
                Auth.GameList.Sort();
                Auth.GameGroupList.Clear();
                Auth.GameGroupList.Add(new GameGroup() { Name = "热门游戏", IsChecked = true });
                Auth.GameGroupList.Add(new GameGroup() { Name = "全部游戏" });
                Auth.GameGroupList.Add(new GameGroup() { Name = "Steam游戏" });
                Auth.GameGroupList.Add(new GameGroup() { Name = "Origin游戏" });
                Auth.GameGroupList.Add(new GameGroup() { Name = "Battle游戏" });
                Auth.GameGroupList.Add(new GameGroup() { Name = "其他游戏" });


                action(new MsgBase() { State = MsgState.Susuccess });
            });
        }

        /// <summary>
        /// 读取服务器消息
        /// </summary>
        /// <param name="action"></param>
        public static void LoadMessage(Action<MsgBase> action)
        {
            Task.Factory.StartNew(() =>
            {
                Auth.NetworkNodeList = new List<NetworkNode>();
                ///调用接口
                string ret = Http.Get("http://www.kailitec.com/api/getAnnounceListJson.php?" + "key=" + Auth.Key);
                //{"id":"0","name":"全球服","group":"HK","picname":"全球服",}
                JObject obj = JsonConvert.DeserializeObject<JObject>(ret);
                JArray ar = (JArray)obj["data"];
                foreach (JObject item in ar)
                {
                    Auth.MessageList.Add(new Message()
                    {
                        Id = item["id"].ToString()
                        ,Announcement = item["announcement"].ToString(),
                        Date = item["date"].ToString(),Title = item["title"].ToString(),
                        Name = "["+item["date"].ToString()+ "]" + item["title"].ToString()
                    });
                }
                action(new MsgBase() { State = MsgState.Susuccess });
            });
        }

        /// <summary>
        /// Ping检测 列表
        /// </summary>
        /// <param name="action"></param>
        public static void PingList(string ip, Action<MsgBase> action)
        {

            //告诉客户端重新刷新UI。
            action(new MsgBase() { State = MsgState.Susuccess });
        }

        /// <summary>
        /// Ping检测 单独
        /// </summary>
        /// <param name="action"></param>
        public static void PingOne(string ip, Action<MsgBase> action)
        {
            Task.Factory.StartNew(() =>
            {
                Ping p = new Ping();
                PingReply pr = p.Send(ip);
                if (pr.Status == IPStatus.Success)
                {
                    string ping = pr.RoundtripTime.ToString();
                    action(new MsgBase()
                    {
                        Message = ping.PadLeft(2, '0') + "ms",
                        Value = Convert.ToInt32(ping)
                    });
                }
                //第一次失败，不返回，重新tcpping
                else
                {
                    string ping = TcpPing(ip);
                    action(new MsgBase()
                    {
                        Message = ping.ToString().PadLeft(2, '0') + "ms", Value = Convert.ToInt32(ping)
                    });
                }
            });
        }

        /// <summary>
        /// TCP - Ping检测
        /// </summary>
        /// <param name="action"></param>
        public static string TcpPing(string ip)
        {
            var socket = new System.Net.Sockets.TcpClient();
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks); //获取当前时间的刻度数
                socket.Connect(ip, 1723);
                if (socket.Connected)
                {
                    TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan ts = ts2.Subtract(ts1).Duration();//时间差的绝对值 ，测试你的代码运行了多长时间。
                    socket.Close();
                    return ts.Milliseconds.ToString();
                }
                else
                {
                    return "999";
                }
            }
            catch
            {
                return "999"; ;
            }
        }
    }
}
