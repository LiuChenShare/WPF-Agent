using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

/// <summary>
/// DMSkin专用HTTP封装类
/// </summary>
namespace DMSkin.API
{
    #region 用户认证
    public class Auth
    {
        /// <summary>
        /// 认证Token
        /// </summary>
        public static string Token;

        /// <summary>
        /// 用户登录名
        /// </summary>
        public static string LoginName;

        /// <summary>
        /// 用户登录密码
        /// </summary>
        public static string LoginPwd;

        /// <summary>
        /// 用户唯一ID
        /// </summary>
        public static string UserId;

        /// <summary>
        /// 用户操作密码
        /// </summary>
        public static string OperatePwd;

        /// <summary>
        /// 用户名
        /// </summary>
        public static string UserName;

        /// <summary>
        /// 角色ID
        /// </summary>
        public static string RoleId;
    }
    #endregion

    #region 服务器配置
    public class ServicePath
    {
        //public static string Path = "localhost:52935";
        static string Path = "club.dmskin.com";


        public static string GetPath
        {
            get { return Path; }
            set { Path = value; }
        }
        /////服务端
        /// <summary>
        /// 登录接口
        /// </summary>
        public static string Ashx = "http://" + GetPath + "/API.ashx";
    }
    #endregion

    #region 实体类
    public class ImageTask
    {
        public string ImageUrl;
        public Uri Image;
        public Action<string> action;
    }
    #endregion

    #region HTTP访问
    public class HTTP
    {
        /// <summary>
        /// 浏览器标识
        /// </summary>
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        /// <summary>
        /// 超时时间
        /// </summary>
        private static int timeout = 15000;//超时

        /// <summary>
        /// 封装参数
        /// </summary>
        private static void AddPar(Dictionary<string, string> parameters)
        {
            //封装参数
            parameters.Add("token", Auth.Token);//用户token
            parameters.Add("userid", Auth.UserId);//用户id
        }

        /// <summary>
        /// 打开线程请求并且回调
        /// </summary>
        /// <param name="par">参数集合</param>
        /// <param name="posturl">目标地址</param>
        /// <param name="target">目标函数</param>
        /// <param name="action">回调函数</param>
        /// <param name="erroraction">错误回调</param>
        public static void Open(Dictionary<string, string> par,string posturl,string target, Action<string> action,Action erroraction)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                par.Add("action", target);
                #region 调用网络
                string json = "";
                try
                {
                    json = Post(posturl, par);
                    action(json);
                }
                catch (Exception ex)//全局错误-网络错误 操作错误
                {
                    Console.WriteLine(ex.Message);
                    erroraction();
                }
                #endregion
            }))
            {
                IsBackground = true
            };
            thread.Start();
        }


        public static string Post(string postUrl, Dictionary<string, string> parameters)
        {
            AddPar(parameters);
            string retString = "";
            using (HttpWebResponse response = CreatePostHttpResponse(postUrl, parameters))
            {
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("UTF-8"));
                retString = myStreamReader.ReadToEnd();
                //后开先关
                myStreamReader.Close();
            }
            return retString;
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            //request.Host = host;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = DefaultUserAgent;
            request.Timeout = timeout;
            //request.CookieContainer=cookies;

            //如果需要POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 下载图片并且返回Uri
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="image"></param>
        internal static void OpenImage(string imageUrl,Action<string> action)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                using (WebClient wb = new WebClient())
                {
                    if (!Directory.Exists("images"))
                    {
                        Directory.CreateDirectory("images");
                    }
                    string fileNameTime = Guid.NewGuid().ToString();
                    //string fileNameTime = DateTime.Now.ToString("yyyyMMddhhmmss");
                    string name = Directory.GetCurrentDirectory() + "\\images\\" + fileNameTime + ".jpg";
                    wb.DownloadFile(imageUrl, name);
                    //wb.Dispose();
                    //初始化图片地址
                    action(name);
                }
            }))
            {
                IsBackground = true
            };
            thread.Start();
        }

        /// <summary>
        /// 批量下载绑定
        /// </summary>
        /// <param name="imageUrlList"></param>
        internal static void OpenImage(List<ImageTask> imageUrlList)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                using (WebClient wb = new WebClient())
                {
                    if (!Directory.Exists("images"))
                    {
                        Directory.CreateDirectory("images");
                    }
                    foreach (var item in imageUrlList)//批量下载
                    {
                        string fileNameTime = Guid.NewGuid().ToString();
                        //string fileNameTime = DateTime.Now.ToString("yyyyMMddhhmmss");
                        string imageurl = Directory.GetCurrentDirectory() + "\\images\\" + fileNameTime + ".jpg";
                        wb.DownloadFile(item.ImageUrl, imageurl);
                        //初始化图片地址
                        item.action(imageurl);
                    }
                }
            }))
            {
                IsBackground = true
            };
            thread.Start();
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.Default.GetBytes(str);
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }
            return (sb.ToString());
        }
    }
    #endregion
}
