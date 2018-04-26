using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinKu.Titanium
{
    /// <summary>
    /// 域名匹配规则
    /// </summary>
    public class DomainRules
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 正则
        /// </summary>
        public List<string> RexStr { get; set; }

        /// <summary>
        /// 是否统配
        /// </summary>
        public bool Wildcard { get; set; }
    }

    /// <summary>
    /// 域名匹配规则列表
    /// </summary>
    public class Rules
    {
        //public List<DomainRules> DomainRules { get; set; }

        /// <summary>
        /// 白名单(哈希表)
        /// </summary>
        public Hashtable WhiteRules { get; set; }
        /// <summary>
        /// 带通配符的白名单
        /// </summary>
        public List<DomainRules> WhiteWildcardRules { get; set; }

        /// <summary>
        /// 黑名单
        /// </summary>
        public Hashtable BlackRules { get; set; }
        /// <summary>
        /// 带通配符的黑名单
        /// </summary>
        public List<DomainRules> BlackWildcardRules { get; set; }

        private static Rules _rules = null;

        /// <summary>
        /// 获取规则信息
        /// </summary>
        /// <param name="refresh">是否刷新</param>
        /// <returns></returns>
        public static Rules GetRules(bool refresh = false)
        {
            if (_rules == null || refresh)
            {
                _rules = new Rules();
                _rules.WhiteRules = new Hashtable();
                _rules.WhiteWildcardRules = new List<DomainRules>();
                _rules.BlackRules = new Hashtable();
                _rules.BlackWildcardRules = new List<DomainRules>();

                FileJson fileJson = new FileJson();
                var fileModel = fileJson.GetResult("RuleList");
                var whiteResult = (fileModel.Whitelist ?? new List<DomainRules>()).ToRulesResult();
                var blackResult = (fileModel.Blacklist ?? new List<DomainRules>()).ToRulesResult();

                foreach (var item in whiteResult.Rules)
                {
                    _rules.WhiteRules.Add(item.Host, item.RexStr);
                }
                _rules.WhiteWildcardRules = whiteResult.WildcardRules;
                foreach (var item in blackResult.Rules)
                {
                    _rules.BlackRules.Add(item.Host, item.RexStr);
                }
                _rules.BlackWildcardRules = blackResult.WildcardRules;
            }
            return _rules;
        }
    }

    /// <summary>
    /// 通过读取文件获取的规则信息
    /// </summary>
    public class RulesResult
    {
        /// <summary>
        /// 全配规则
        /// </summary>
        public List<DomainRules> Rules { get; set; }
        /// <summary>
        /// 通配规则
        /// </summary>
        public List<DomainRules> WildcardRules { get; set; }
    }

    /// <summary>
    /// 保存在文件中的规则信息
    /// </summary>
    public class RulesFile
    {
        /// <summary>
        /// 白名单
        /// </summary>
        public List<DomainRules> Whitelist { get; set; }
        /// <summary>
        /// 黑名单
        /// </summary>
        public List<DomainRules> Blacklist { get; set; }
    }
}
