using LinKu.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LinKu
{
    public class Http
    {
        /// <summary>
        /// 浏览器
        /// </summary>
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;
        //public static  string Host = "";
        private static readonly int TimeOut = 25000;
        private static readonly CookieContainer Cookies = new CookieContainer();

        /// <summary>
        /// Post
        /// </summary>
        /// <param name="postUrl">地址</param>
        /// <param name="parameters">参数集合</param>
        /// <param name="action">回调</param>
        public static void Post(string postUrl, Dictionary<string, string> parameters, Action<MsgBase> action)
        {
            Task.Factory.StartNew(() =>
            {
                using (HttpWebResponse response = PostFunc(postUrl, parameters))
                {
                    try
                    {
                        Stream myResponseStream = response.GetResponseStream();
                        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("UTF-8"));
                        string retString = myStreamReader.ReadToEnd();
                        //后开先关
                        myStreamReader.Close();
                        //myResponseStream.Close();
                        action(new MsgBase() { Message = retString });
                    }
                    catch (Exception)
                    {
                        action(new MsgBase() { State = MsgState.Error });
                    }
                }
            });
        }
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="getUrl">地址</param>
        /// <param name="action">回调</param>
        public static string Get(string getUrl)
        {
            using (HttpWebResponse response = GetFunc(getUrl))
            {
                try
                {
                    Stream myResponseStream = response.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("UTF-8"));
                    string retString = myStreamReader.ReadToEnd();
                    //后开先关
                    myStreamReader.Close();
                    //myResponseStream.Close();
                    return retString;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        private static HttpWebResponse PostFunc(string url, Dictionary<string, string> parameters)
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
            //request.Host = Host;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = DefaultUserAgent;
            request.Timeout = TimeOut;
            request.CookieContainer = Cookies;

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
                byte[] data = DefaultEncoding.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }
        private static HttpWebResponse GetFunc(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.Headers.Add("Accept-Charset", "GBK,utf-8;q=0.7,*;q=0.3");
            request.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            request.Headers.Add("Cache-Control", "max-age=0");
            //request.Host = Host;
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = DefaultUserAgent;
            request.Timeout = TimeOut;
            request.CookieContainer = Cookies;
            try
            {
                return request.GetResponse() as HttpWebResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        //使用手册
        //返回加密的GZIP
        //HttpWebResponse response = HttpHelper.CreateGetHttpResponse(url, 2500, null, null);
        //   GZipStream g = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
        //   System.IO.StreamReader r = new System.IO.StreamReader(g,Encoding.UTF8);
        //   string retString = r.ReadToEnd();
        //   //后开先关
        //   r.Close();
        //   r.Dispose();
        //   g.Close();
        //   g.Dispose();
        //   //处理返回的JSON



        //没有加密的时候
        //IDictionary<string, string> parameters = new Dictionary<string, string>();
        //    //下面是发送的数据
        //    parameters.Add("para",str);
        //    //para:1
        //    HttpWebResponse response =  HttpHelper.CreatePostHttpResponse(postUrl,parameters,25000,UserAgent,Encoding.UTF8,new System.Net.CookieContainer(),refer,host);
        //    Stream myResponseStream = response.GetResponseStream();
        //    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("UTF-8"));
        //    string retString = myStreamReader.ReadToEnd();
        //    //后开先关
        //    myStreamReader.Close();
        //    myStreamReader.Dispose();
        //    myResponseStream.Close();
        //    myResponseStream.Dispose();
        //    return retString;
    }
}
