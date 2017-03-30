using ComLibrary;
using HttpLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace SdjLibrary
{
    public class SdjRequest : BaseRequest
    {

        LogUtil log = new LogUtil();

        public SdjRequest(Dictionary<string, string> requestDic, string mchId, string mchName, string pfxPath, string pfxPwd, string cerPath, string orgKey)
            : base(requestDic, mchId, mchName, pfxPath, pfxPwd, cerPath, orgKey)
        {

        }

        public override BaseResponse doCollection()
        {
            BaseResponse sdjResponse = new SdjResponse();

            Dictionary<string, string> reqParam = new Dictionary<string, string>();
            reqParam=this.requestDic;
            string signType = requestDic["signType"].ToString();
            reqParam.Add("payeeId", this.mchId);
            reqParam.Remove("sign");
            reqParam.Remove("signType");
            reqParam.Remove("signature");
            string sdjDKLinkStr = SdjUtil.createSdjDKLinkStr(reqParam);
            string md5Str = sdjDKLinkStr + "&merKey=" + orgKey;
            log.Write("盛迪嘉代扣待加签德字符串:" + md5Str);
            string md5Sign = Md5Util.encrypt(md5Str);
            //证书加密数据
            string encryptStr = SdjUtil.cerEncrypt(md5Sign, this.cerPath);
            //证书签名数据
            string sign = SdjUtil.sign(encryptStr, this.pfxPath, this.pfxPwd);
            reqParam.Add("signType", signType);
            reqParam.Add("signature", encryptStr);
            reqParam.Add("sign", sign);

            NameValueCollection nvc = new NameValueCollection();
            foreach (string key in reqParam.Keys)
            {
                nvc.Add(key, reqParam[key]);
            }
            string serverUrl = "http://183.11.223.20:12586/api/commNewDk/CommDkNewPay";
            string responseStr = HttpRequest.HttpPost(serverUrl, nvc, Encoding.UTF8);
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            Dictionary<string, string> dicRslt;
            dicRslt = jsonSer.Deserialize<Dictionary<string, string>>(responseStr);
            //{"execCode":"000000","orderNo":"1234567819922","tradeNo":"2017032900010000000000733188","execMsg":"订单交易成功","poundage":"0.6","payeeId":"8619143953"}
            sdjResponse.orderId = dicRslt.ContainsKey("orderNo") ? dicRslt["orderNo"] : "";
            sdjResponse.respCode = dicRslt.ContainsKey("execCode") ? dicRslt["execCode"] : "";
            sdjResponse.respMsg = dicRslt.ContainsKey("execMsg") ? dicRslt["execMsg"] : "";
            sdjResponse.bankOrderNo = dicRslt.ContainsKey("tradeNo") ? dicRslt["tradeNo"] : "";
            log.Write("respCode[" + sdjResponse.respCode + "]" + "respDesc[" + sdjResponse.respMsg + "]");

            sdjResponse.status = "0";

            if ("000000".Equals(sdjResponse.respCode))
            {
                sdjResponse.returnCode = "0000";
                sdjResponse.returnMsg = "提交成功";
                sdjResponse.status = "1";
            }
            else if ("000010".Equals(sdjResponse.respCode))
            {
                sdjResponse.returnCode = "0000";
                sdjResponse.returnMsg = "提交成功";
            }
            else
            {
                sdjResponse.returnCode = "9999";
                sdjResponse.returnMsg = sdjResponse.respMsg;
            }
            return sdjResponse;

        }
        public override BaseResponse doQuery()
        {
            BaseResponse sdjResponse = new SdjResponse();
            Dictionary<string, string> reqParam = new Dictionary<string, string>();
            string orderNo = this.requestDic.ContainsKey("orderNo") ? this.requestDic["orderNo"] : "";
            reqParam.Add("serviceType", "agentPayQuery");
            reqParam.Add("payeeId", this.mchId);
            reqParam.Add("interfaceVersion", "V1.0");
            reqParam.Add("queryType", "1");
            reqParam.Add("signType", "MD5");
            reqParam.Add("inputCharset", "UTF-8");
            reqParam.Add("orderNo", orderNo);
            reqParam.Add("tradeNo", "");
            string sdjQueryLinkStr = SdjUtil.createSdjQueryLinkStr(reqParam);
            string md5Str = sdjQueryLinkStr + "&merKey=" + orgKey;
            log.Write("盛迪嘉查询待加签德字符串:" + md5Str);
            string md5Sign = Md5Util.encrypt(md5Str);
            //证书加密数据
            string signature = SdjUtil.cerEncrypt(md5Sign, this.cerPath);
            //证书签名数据
            string sign = SdjUtil.sign(signature, this.pfxPath, this.pfxPwd);
            reqParam.Add("signature", signature);
            reqParam.Add("sign", sign);

            NameValueCollection nvc = new NameValueCollection();
            foreach (string key in reqParam.Keys)
            {
                nvc.Add(key, reqParam[key]);
            }
            string serverUrl = "http://183.11.223.20:12586/api/commNewDk/CommDkNewPay";
            string responseStr = HttpRequest.HttpPost(serverUrl, nvc, Encoding.UTF8);
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            Dictionary<string, string> dicRslt;
            dicRslt = jsonSer.Deserialize<Dictionary<string, string>>(responseStr);
            //{"execCode":"000000","orderNo":"1234567819922","tradeNo":"2017032900010000000000733188","execMsg":"订单交易成功","poundage":"0.6","payeeId":"8619143953"}
            string isSuccess = dicRslt.ContainsKey("IsSuccess") ? dicRslt["IsSuccess"] : "";
            sdjResponse.orderId = dicRslt.ContainsKey("orderNo") ? dicRslt["orderNo"] : "";
            sdjResponse.respCode = dicRslt.ContainsKey("execCode") ? dicRslt["execCode"] : "";
            sdjResponse.respMsg = dicRslt.ContainsKey("execMsg") ? dicRslt["execMsg"] : "";
            sdjResponse.bankOrderNo = dicRslt.ContainsKey("tradeNo") ? dicRslt["tradeNo"] : "";
            log.Write("respCode[" + sdjResponse.respCode + "]" + "respDesc[" + sdjResponse.respMsg + "]");

            sdjResponse.status = "0";
            if ("T".Equals(isSuccess))
            {
                if ("000000".Equals(sdjResponse.respCode))
                {
                    sdjResponse.returnCode = "0000";
                    sdjResponse.returnMsg = "代扣成功";
                    sdjResponse.status = "1";
                }
                //else if ("000010".Equals(sdjResponse.respCode))
                //{
                //    sdjResponse.returnCode = "0000";
                //    sdjResponse.returnMsg = "提交成功";
                //}
                else if("100000".Equals(sdjResponse.respCode)){
                    sdjResponse.returnCode = "9999";
                    sdjResponse.returnMsg = sdjResponse.respMsg;
                    sdjResponse.status = "2";
                }
                else//进行中
                {
                    sdjResponse.returnCode = "9999";
                    sdjResponse.returnMsg = sdjResponse.respMsg;
                }
            }
            return sdjResponse;
        }
    }
}
