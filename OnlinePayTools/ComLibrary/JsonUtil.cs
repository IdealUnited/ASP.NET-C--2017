using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace ComLibrary
{
    public static class JsonUtil
    {

        // 从一个对象信息生成Json串
        public static string ObjectToJson(object obj)
        {
                       //报文体json
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            return jsonSer.Serialize(obj);
            
        }
        // 从一个Json串生成对象信息
        public static object JsonToObject(string jsonString, object obj)
        {
            return null;
            //JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            //return jsonSer.Deserialize<Activator.CreateInstance(obj)>(jsonString);
        }
    }
}
