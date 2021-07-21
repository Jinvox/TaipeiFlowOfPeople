using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeopleModel
{
    public class CommunityHelper
    {
        private static CommunityHelper _instance;
        public static CommunityHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CommunityHelper();
                return _instance;
            }
        }
        
        private CommunityHelper()
        {
        }

        public string GetJsonContent(string Url, bool isAddHeader)
        {
            string targetURI = Url;
            var request = System.Net.WebRequest.Create(targetURI);
            request.ContentType = "application/json; charset=utf-8";
            if (isAddHeader)
            {
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                request.Headers.Add("Host", "ptx.transportdata.tw");
            }

            var response = request.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }
            return text;
        }

        public string GetJsonContentYouBike(string Url)
        {
            string targetURI = Url;
            var request = System.Net.WebRequest.Create(targetURI);
            request.ContentType = "application/json; charset=utf-8";
            var response = request.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            JObject json = JObject.Parse(text);
            JObject coins = (JObject)json["retVal"];
            JArray array = new JArray();
            foreach (JProperty property in coins.Properties())
            {
                string name = property.Name;
                array.Add(property.Value);
            }
            json["retVal"] = array;
            text = json.ToString();
            return text;
        }
    }
}
