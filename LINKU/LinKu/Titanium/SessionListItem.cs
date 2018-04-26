using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.Examples.Wpf.Annotations;
using Titanium.Web.Proxy.Http;

namespace LinKu.Titanium
{
    public class SessionListItem : INotifyPropertyChanged
    {
        private string statusCode;
        private string protocol;
        private string host;
        private string url;
        private long? bodySize;
        private string process;
        private long receivedDataCount;
        private long sentDataCount;
        private Exception exception;
        private string sentExt;
        private string sentRowColor;
        private string sentpRex;
        private string sentpHost;
        private string sentpExt;
        private string sentpPass;
        private string sentpUse;

        public int Number { get; set; }

        public HttpWebClient WebSession { get; set; }

        public bool IsTunnelConnect { get; set; }

        public string StatusCode
        {
            get => statusCode;
            set => SetField(ref statusCode, value);
        }

        public string Protocol
        {
            get => protocol;
            set => SetField(ref protocol, value);
        }

        public string Host
        {
            get => host;
            set => SetField(ref host, value);
        }

        public string Url
        {
            get => url;
            set => SetField(ref url, value);
        }

        public long? BodySize
        {
            get => bodySize;
            set => SetField(ref bodySize, value);
        }

        public string Process
        {
            get => process;
            set => SetField(ref process, value);
        }

        public long ReceivedDataCount
        {
            get => receivedDataCount;
            set => SetField(ref receivedDataCount, value);
        }

        public long SentDataCount
        {
            get => sentDataCount;
            set => SetField(ref sentDataCount, value);
        }

        public string Ext
        {
            get => sentExt;
            set => SetField(ref sentExt, value);
        }


        public Exception Exception
        {
            get => exception;
            set => SetField(ref exception, value);
        }

        public string pRex
        {
            get => sentpRex;
            set => SetField(ref sentpRex, value);
        }


        public string pHost
        {
            get => sentpHost;
            set => SetField(ref sentpHost, value);
        }

        public string pPass
        {
            get => sentpPass;
            set => SetField(ref sentpPass, value);
        }

        public string pUse
        {
            get => sentpUse;
            set => SetField(ref sentpUse, value);
        }

        public string RowColor
        {
            get => sentRowColor;
            set => SetField(ref sentRowColor, value);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Update()
        {
            var request = WebSession.Request;
            var response = WebSession.Response;
            int statusCode = response?.StatusCode ?? 0;
            StatusCode = statusCode == 0 ? "-" : statusCode.ToString();
            Protocol = request.RequestUri.Scheme;

            if (IsTunnelConnect)
            {
                Host = "Tunnel to";
                Url = request.RequestUri.Host + ":" + request.RequestUri.Port;
            }
            else
            {
                Host = request.RequestUri.Host;
                Url = request.RequestUri.AbsolutePath;
            }


            if (!IsTunnelConnect)
            {
                long responseSize = -1;
                if (response != null)
                {
                    if (response.ContentLength != -1)
                    {
                        responseSize = response.ContentLength;
                    }
                    else if (response.IsBodyRead && response.Body != null)
                    {
                        responseSize = response.Body.Length;
                    }
                }

                BodySize = responseSize;
            }

            Process = GetProcessDescription(WebSession.ProcessId.Value);

            //这里应该再次验证该请求有没有走通道，并给sentpUse赋值
            //pUse = "✔" or "✘" or "-" 
            Rules _rules = Rules.GetRules();
            DomainMethod domain = new DomainMethod();

            var blockadeRules = domain.GetBlockadeRules(request.RequestUri.Host, request.RequestUri.AbsolutePath);
            if (blockadeRules.RexStr != null && blockadeRules.RexStr.Count > 0)
            {
                //说明进了黑名单匹配
                pUse = "✘";
            }
            else
            {
                var adoptRules = domain.GetAdoptRules(request.RequestUri.Host, request.RequestUri.AbsolutePath);
                if (adoptRules.RexStr != null && adoptRules.RexStr.Count > 0)
                {
                    //说明通过了白名单匹配
                    pUse = "✔";
                }
                else
                {
                    pUse = "-";
                }
            }
        }


        private string GetProcessDescription(int processId)
        {
            try
            {
                var process = System.Diagnostics.Process.GetProcessById(processId);
                return process.ProcessName + ":" + processId;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
