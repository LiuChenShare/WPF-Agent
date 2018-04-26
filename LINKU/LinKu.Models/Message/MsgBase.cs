using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinKu.Models
{
    /// <summary>
    /// 消息基类
    /// </summary>
    public class MsgBase
    {
        //状态 - 默认状态 为成功
        private MsgState _state = MsgState.Susuccess;
        /// <summary>
        /// 消息标识状态
        /// </summary>
        public MsgState State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
            }
        }


        /// <summary>
        /// 具体的报错信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public int Value { get; set; }
    }
}
