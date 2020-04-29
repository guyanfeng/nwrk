using System;
using System.Collections.Generic;
using System.Net.Http;

namespace nwrk.app
{
    /// <summary>
    /// 测试
    /// </summary>
    public class NWrkHttpTestWorker : NWrkHttpWorker
    {
        protected override string[] ParseResponse(string[] input, string content)
        {
            var rep = Newtonsoft.Json.JsonConvert.DeserializeObject<NWrkResponse>(content);
            return new[]
            {
                rep.id,
                rep.name,
                rep.result
            };
        }

        protected override HttpContent CreateHttpContent(string[] record)
        {
            return new FormUrlEncodedContent(
                new[]{
                    new KeyValuePair<string,string>("id", record[0]),
                    new KeyValuePair<string, string>("name", record[1])
                });
        }
    }

    public class NWrkResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string result { get; set; }
    }
}
