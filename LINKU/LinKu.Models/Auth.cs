using LinKu.Models;
using LinKu.Models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinKu
{
    /// <summary>
    /// 用户认证类
    /// </summary>
    public class Auth
    {
        public static string Home = "http://www.dmskin.com";

        public static bool IsLogin = false;

        public static InternalClass InternalClass;

        public static string Key = "qwertyuioplkjhgfdsazxcvbnm";

        public static User user;

        public static Game SelectGame = new Game() { Name="未选择" };//H1Z1北美 H1Z1%E5%8C%97%E7%BE%8E

        public static NetworkNode SelectNetworkNode = new NetworkNode() { Name = "未选择" };//H1Z1北美 H1Z1%E5%8C%97%E7%BE%8E

        /// <summary>
        /// 游戏加速节点列表
        /// </summary>
        public static List<NetworkNode> NetworkNodeList = new List<NetworkNode>();

        /// <summary>
        /// 游戏列表
        /// </summary>
        public static List<Game> GameList = new List<Game>();

        /// <summary>
        /// 游戏消息
        /// </summary>
        public static List<Message> MessageList = new List<Message>();

        /// <summary>
        /// 游戏分组
        /// </summary>
        public static List<GameGroup> GameGroupList = new List<GameGroup>();
    }
}
