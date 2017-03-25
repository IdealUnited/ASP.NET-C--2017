using ComLibrary;
using HttpLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SdjLibrary
{
    public class SdjRequest:BaseRequest
    {

        LogUtil log = new LogUtil();

        public SdjRequest(Dictionary<string, string> requestDic, string mchId, string mchName, string pfxPath, string pfxPwd, string cerPath, string orgKey)
            : base(requestDic, mchId, mchName, pfxPath, pfxPwd, cerPath, orgKey)
        {

        }

        public override BaseResponse doCollection()
        {
            BaseResponse sdjResponse = new SdjResponse();

            Dictionary<string, string> reqParam = this.requestDic;
            string signType = requestDic["signType"].ToString();
            reqParam.Add("payeeId", this.mchId);
            reqParam.Remove("sign");
            reqParam.Remove("signType");
            reqParam.Remove("signature");
            string md5Sign = SdjUtil.generateMd5Sign(requestDic, this.orgKey);
            //证书加密数据
            string encryptStr = SdjUtil.cerEncrypt(md5Sign, this.cerPath);
            //证书签名数据
            string sign = SdjUtil.sign(encryptStr,this.pfxPath, this.pfxPwd);
            reqParam.Add("signType", signType);
            reqParam.Add("signature", encryptStr);
            reqParam.Add("sign", sign);
            string serverUrl="http://183.11.223.20:12586/api/commNewDk/CommDkNewPay";
            string responseStr = HttpRequest.post(serverUrl, reqParam);


            return sdjResponse;
        }


        public override ComLibrary.BaseResponse doQuery()
        {
            BaseResponse sdjResponse = new SdjResponse();

            return sdjResponse;
        }
    }
}
