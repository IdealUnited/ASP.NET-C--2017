using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ComLibrary
{
    public class BaseRequest
    {
        public Dictionary<string, string> requestDic;
        public string mchId;
        public string mchName;
        public string pfxPath; 
        public string pfxPwd; 
        public string cerPath;
        public string orgKey;

        public BaseRequest(Dictionary<string, string> requestDic,string mchId, string mchName, string pfxPath, string pfxPwd, string cerPath,string orgKey)
        {
            this.requestDic = requestDic;
            this.mchId = mchId;
            this.mchName = mchName;
            this.pfxPath = pfxPath;
            this.pfxPwd = pfxPwd;
            this.cerPath = cerPath;
            this.orgKey = orgKey;
        }
        public virtual BaseResponse doCollection() { return new BaseResponse(); }
        public virtual BaseResponse doAgentPay() { return new BaseResponse(); }
        public virtual BaseResponse doQuery() { return new BaseResponse(); }
    }
}
