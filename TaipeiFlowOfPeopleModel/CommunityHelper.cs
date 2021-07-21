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

        public string GetJsonContent(string Url)
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
            return text;
        }
    }
}
