using System;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace nwrk.app
{
    public class NWrkHttpWorker : NWrkWorker
    {
        protected readonly HttpClient _http;
        protected Regex _reg_data = new Regex("{(\\d+)}", RegexOptions.Compiled);

        public string ContentType { get; set; }

        /// <summary>
        /// 数据模版
        /// </summary>
        public string Data { get; set; }

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

        protected virtual string[] ParseResponse(string[] input, string content)
        {
            var list = new List<string>(input);
            list.Add(content);
            return list.ToArray();
        }

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
            var text = CreateData(record);
            var content = new StringContent(text, Encoding.UTF8, ContentType);
            return content;
        }

        protected virtual string CreateParams(string[] record)
        {
            return "?" + CreateData(record);
        }

        string CreateData(string[] record)
        {
            var data = Data;
            foreach (Match m in _reg_data.Matches(Data))
            {
                var index = Convert.ToInt32(m.Groups[1].Value);
                data = data.Replace($"{{{index}}}", record[index]);
            }
            return data;
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

            Data = section["data"];

            ContentType = section["contentType"];
        }
    }
}