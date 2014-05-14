using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace NightOwl
{
    public static class Json2XML
    {
        public static XmlDocument J2X(string json,string rootName)
        {
            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json,rootName);
            return doc;
        }
    }
}
