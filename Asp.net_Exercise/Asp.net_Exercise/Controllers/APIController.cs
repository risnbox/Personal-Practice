using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;

namespace Asp.net_Exercise.Controllers
{
    public class APIController : ApiController
    {
        public string Get(string city,string town)
        {
            
            var url = "https://emap.pcsc.com.tw/EMapSDK.aspx?commandid=SearchStore&city=" + city + "&town=" + town;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream stream = null;
            string xml = "";
            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                response = request.GetResponse() as HttpWebResponse;
                stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                xml = streamReader.ReadToEnd();
                streamReader.Close();
                stream.Close();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);
                return json;
            }
            catch (Exception e)
            {
                return ("請求失敗" + e);
            }
            
        }
    }
}
