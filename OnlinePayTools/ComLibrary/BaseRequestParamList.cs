using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLibrary
{
    public class BaseRequestParamList
    {
        public virtual Dictionary<string, object> SetKeyValue() { Dictionary<string, object> ParamDiclist = new Dictionary<string, object>(); return ParamDiclist; }
        public Dictionary<string, object> GetKeyValue() { return SetKeyValue(); }
        public virtual string resetReqNoAndReqTime() { return ""; }
    }
}
