using System;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace nwrk.app
{
    public abstract class NWrkHttpWorker : NWrkWorker
    {
        protected readonly HttpClient _http;

        /// <summary>
        /// 根 url
        /// </summary>
        public Uri BaseUrl { get
            {
                return _http.BaseAddress;
            }
            set
            {
                _http.BaseAddress = value;
            }
        }

        /// <summary>
        /// 相对 url
        /// </summary>
        public string RelUrl { get; set; }

        /// <summary>
        /// Post 提交数据
        /// </summary>
        public HttpContent PostContent { get; set; }

        protected abstract string[] ParseResponse(string[] input, string content);

        public string Method
        {
            get;set;
        }

        public NWrkHttpWorker()
        {
            _http = new HttpClient();
            
        }

        protected virtual HttpContent CreateHttpContent(string[] record)
        {
            return null;
        }

        protected virtual string CreateParams(string[] record)
        {
            return null;
        }
            
        protected override async Task<string[]> ExecuteReader(string[] record)
        {
            HttpResponseMessage response = null;
            switch (Method.ToLower())
            {
                case "get":
                    response = await _http.GetAsync(RelUrl + CreateParams(record));
                    break;
                case "post":
                    response = await _http.PostAsync(RelUrl, CreateHttpContent(record));
                    break;
                default:
                    break;
            }
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var repRecord = ParseResponse(record, content);
            return repRecord;
        }

        public override void ReadConfig(IConfigurationSection section)
        {
            base.ReadConfig(section);

            BaseUrl = new Uri(section["baseUrl"]);

            RelUrl = section["relUrl"];

            Method = section["method"];
        }
    }
}