using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Titanium.Web.Proxy.Examples.Wpf
{
    public class FileJson
    {
        /// <summary>
        /// 读取规则配置文件
        /// </summary>
        /// <returns></returns>
        public RulesResult GetResult(string path)
        {
            RulesResult rulesResult = new RulesResult();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string filepath = basePath + "/" + path + ".json";
            var result = GetFileJson(filepath);

            List<DomainRules> list = JsonConvert.DeserializeObject<List<DomainRules>>(result);
            var wildcard = list.Where(x => x.Host.StartsWith("*")).Select(x => x.ToWildcardRules()).ToList();
            rulesResult.Rules = list;
            rulesResult.WildcardRules = wildcard;

            return rulesResult;
        }

        /// <summary>
        /// 写入配置文件  
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(List<DomainRules> result)
        {
            //string basePath = AppDomain.CurrentDomain.BaseDirectory;
            //string filepath = basePath + "/Model.json";
            //var infos = GetResult();
            //foreach (var item in result)
            //{
            //    if (infos.Where(x => x.Host == item.Host).FirstOrDefault() != null)
            //    {
            //        infos.Where(x => x.Host == item.Host).FirstOrDefault().RexStr = item.RexStr;
            //    }
            //    else
            //    {
            //        infos.Add(item);
            //    }
            //}
            //string json = JsonConvert.SerializeObject(infos);
            //SetFileJson(filepath, json);
        }

        /// <summary>
        /// 读取json文件
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private string GetFileJson(string filepath)
        {
            string json = string.Empty;
            using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, System.IO.FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("gb2312")))
                {
                    json = sr.ReadToEnd().ToString();
                }
            }
            return json;
        }

        /// <summary>
        /// 写入josn文件
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="json"></param>
        private void SetFileJson(string filepath, string json)
        {
            if (json != null)
            {
                //if (!Directory.Exists(filepath))//没有就创建
                //{
                //    Directory.CreateDirectory(filepath);
                //}
                File.WriteAllText(filepath, json);
            }
        }
    }
}
