using ComLibrary;
using CryptUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Script.Serialization;

namespace SandLibrary
{
    public class SandRequest:BaseRequest
    {

        LogUtil log = new LogUtil();

        public SandRequest(Dictionary<string, string> requestDic,string mchId, string mchName, string pfxPath, string pfxPwd, string cerPath, string orgKey)
            : base(requestDic, mchId, mchName, pfxPath, pfxPwd, cerPath, orgKey)
        {

        }

        public override BaseResponse doCollection()
        {
            BaseResponse sandResponse = new SandResponse();
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            Dictionary<string, string> dicRslt;
            MessageCryptWorker.trafficMessage resp = CollectionMessage(this.requestDic, this.mchId, this.pfxPath, this.pfxPwd, this.cerPath);
            //检查验签结果
            log.Write("验签结果" + resp.sign);
            //解析报文，读取业务报文体内具体字段的值
            log.Write(resp.encryptData, MsgType.Information);
            dicRslt = jsonSer.Deserialize<Dictionary<string, string>>(resp.encryptData);
            //dicRslt = (Dictionary<string, string>)JsonUtil.JsonToObject(resp.encryptData, dicRslt);
            sandResponse.orderId= dicRslt.ContainsKey("orderCode") ? dicRslt["orderCode"] : "";
            sandResponse.respCode = dicRslt.ContainsKey("respCode") ? dicRslt["respCode"] : "";
            sandResponse.respMsg = dicRslt.ContainsKey("respDesc") ? dicRslt["respDesc"] : "";
            sandResponse.bankOrderNo = dicRslt.ContainsKey("sandSerial") ? dicRslt["sandSerial"] : "";
            string resultFlag = dicRslt.ContainsKey("resultFlag") ? dicRslt["resultFlag"] : "";
            log.Write("respCode[" + sandResponse.respCode + "]" + "respDesc[" + sandResponse.respMsg + "]");

            sandResponse.status = "0";

            if ("0000".Equals(sandResponse.respCode))
            {
                if ("0".Equals(resultFlag))
                {
                    sandResponse.returnCode = "0000";
                    sandResponse.returnMsg = "处理完成";
                    sandResponse.status = "1";
                }
                else if ("1".Equals(resultFlag))
                {
                    sandResponse.returnCode = "0002";
                    sandResponse.returnMsg = "处理失败";
                    sandResponse.status = "2";
                }
                else {
                    sandResponse.returnCode = "9999";
                    sandResponse.returnMsg = sandResponse.respMsg;
                }
            }
            else {
                sandResponse.returnCode = "9999";
                sandResponse.returnMsg = sandResponse.respMsg;
            }
            return sandResponse;
        }


        public override ComLibrary.BaseResponse doQuery()
        {
            BaseResponse sandResponse = new SandResponse();
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            Dictionary<string, string> dicRslt;
            MessageCryptWorker.trafficMessage resp = QueryOrderMessage(this.requestDic, this.pfxPath, this.pfxPwd, this.cerPath, this.mchId);
            //检查验签结果
            log.Write("验签结果" + resp.sign);
            //解析报文，读取业务报文体内具体字段的值
            log.Write(resp.encryptData, MsgType.Information);
            dicRslt = jsonSer.Deserialize<Dictionary<string, string>>(resp.encryptData);
            //dicRslt = (Dictionary<string, string>)JsonUtil.JsonToObject(resp.encryptData, dicRslt);
            sandResponse.orderId = dicRslt.ContainsKey("orderCode") ? dicRslt["orderCode"] : "";
            sandResponse.respCode = dicRslt.ContainsKey("respCode") ? dicRslt["respCode"] : "";
            sandResponse.respMsg = dicRslt.ContainsKey("respDesc") ? dicRslt["respDesc"] : "";
            sandResponse.bankOrderNo = dicRslt.ContainsKey("sandSerial") ? dicRslt["sandSerial"] : "";
            string resultFlag = dicRslt.ContainsKey("resultFlag") ? dicRslt["resultFlag"] : "";
            log.Write("respCode[" + sandResponse.respCode + "]" + "respDesc[" + sandResponse.respMsg + "]");

            sandResponse.status = "0";
            if ("0000".Equals(sandResponse.respCode))
            {
                if ("0".Equals(resultFlag))
                {
                    sandResponse.status = "1";
                }
                else if ("1".Equals(resultFlag))
                {
                    sandResponse.status = "2";
                }
            }
            return sandResponse;
        }
        /// <summary>
        ///代扣提交
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="mchId"></param>
        /// <param name="pfxPath"></param>
        /// <param name="pfxPwd"></param>
        /// <param name="cerPath"></param>
        /// <returns></returns>
        private MessageCryptWorker.trafficMessage CollectionMessage(Dictionary<string, string> dic, string mchId, string pfxPath, string pfxPwd, string cerPath)
        {
            //报文结构体初始化
            MessageCryptWorker.trafficMessage msgRequestSource = new MessageCryptWorker.trafficMessage();
            //dic.Add("version", "01");
            //dic.Add("cityNo", "010000");
            //dic.Add("certType", "0001");
            //dic.Add("productId", "00000002");
            //dic.Add("purpose", "collection");
            //dic.Add("accNo", "6226220209634996");
            //dic.Add("accName", "TEST");
            //dic.Add("bankInsCode", "48270000");
            //dic.Add("bankName", "世界银行");
            //dic.Add("accAttr", "0");
            //dic.Add("timeOut", "20161115123021");
            //dic.Add("certNo", "321281198702253717");
            //dic.Add("tranTime", "20161114123021");
            //dic.Add("provNo", "010000");
            //dic.Add("phone", "12345678901");
            //dic.Add("cardId", "321281198702253717");
            //dic.Add("tranAmt", "000000000100");
            //dic.Add("orderCode", "201611131000001042");
            //dic.Add("accType", "4");
            //dic.Add("currencyCode", "156");



            //发送类实体化
            MessageCryptWorker worker = new MessageCryptWorker();
            worker.EncodeCode = Encoding.UTF8; //encoding 使用utf8
            worker.PFXFile = pfxPath; //商户pfx证书路径
            worker.PFXPassword = pfxPwd;  //商户pfx证书密码
            worker.CerFile = cerPath; //杉德cer证书路径
            string ServerUrl = "http://61.129.71.103:7970/agent-main/openapi/collection";
            //string ServerUrl = "https://caspay.sandpay.com.cn/agent-main/openapi/collection";
            msgRequestSource.merId = mchId; //商户号
            msgRequestSource.transCode = "RTCO";        //交易代码
            msgRequestSource.extend = "";               //扩展域

            //报文体json
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            msgRequestSource.encryptData = jsonSer.Serialize(dic);
            //msgRequestSource.encryptData = JsonUtil.ObjectToJson(dic);
            //encrytpKey会在发送前加密时自动生成16位的随机字符

            log.Write("待发送报文为：" + msgRequestSource.encryptData);

            MessageCryptWorker.trafficMessage respMessage = worker.postMessage(ServerUrl, msgRequestSource);
            log.Write("服务器返回为：" + respMessage.encryptData);
            return respMessage;

        }

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="pfxFilePath"></param>
        /// <param name="pfxPassword"></param>
        /// <param name="cerFilePath"></param>
        /// <param name="merId"></param>
        /// <returns></returns>
        private MessageCryptWorker.trafficMessage QueryOrderMessage(Dictionary<string, string> dic, string pfxFilePath,
                string pfxPassword,
                string cerFilePath,
                string merId)
        {
            //报文结构体初始化
            MessageCryptWorker.trafficMessage msgRequestSource = new MessageCryptWorker.trafficMessage();
            //发送类实体化
            MessageCryptWorker worker = new MessageCryptWorker();
            worker.EncodeCode = Encoding.UTF8; //encoding 使用utf8

            worker.PFXFile = pfxFilePath; //商户pfx证书路径
            worker.PFXPassword = pfxPassword;  //商户pfx证书密码
            worker.CerFile = cerFilePath; //杉德cer证书路径


            msgRequestSource.merId = merId; //商户号
            msgRequestSource.transCode = "ODQU";        //交易代码
            msgRequestSource.extend = "";               //扩展域

            //报文体json
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            msgRequestSource.encryptData = jsonSer.Serialize(dic);
            //encrytpKey会在发送前加密时自动生成16位的随机字符

            log.Write("待发送报文为：" + msgRequestSource.encryptData);
            string ServerUrl = "http://61.129.71.103:7970/agent-main/openapi/queryOrder";
            //string ServerUrl = "https://caspay.sandpay.com.cn/agent-main/openapi/queryOrder";

            MessageCryptWorker.trafficMessage respMessage = worker.postMessage(ServerUrl, msgRequestSource);

            log.Write("服务器返回为：" + respMessage.encryptData);
            return respMessage;
        }

        /// <summary>
        /// 代付接口提交
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="pfxFilePath"></param>
        /// <param name="pfxPassword"></param>
        /// <param name="cerFilePath"></param>
        /// <param name="merId"></param>
        /// <returns></returns>
        private MessageCryptWorker.trafficMessage AgentPayMessage(Dictionary<string, string> dic, string pfxFilePath,
        string pfxPassword,
        string cerFilePath,
        string merId)
        {
            //dic.Add("version", "01");
            //dic.Add("productId", "00000003");// 代收对公    00000001 代收对私    00000002  代付对私    00000004 
            //dic.Add("tranTime", "20161114123021");
            //dic.Add("orderCode", "20161113000000001038");
            //dic.Add("timeOut", "20161115123021");
            //dic.Add("tranAmt", "000000000001");
            //dic.Add("currencyCode", "156");
            //dic.Add("accAttr", "0");
            //dic.Add("accType", "2");
            //dic.Add("accNo", "6216261000000000018");
            //dic.Add("accName", "啊啊");
            //dic.Add("bankName", "aaa");
            //dic.Add("bankType", "1234567890");
            //dic.Add("remark", "pay");
            //dic.Add("reqReserved", "");
            //dic.Add("noticeUrl", "");
            //dic.Add("extend", "");
            //报文结构体初始化
            MessageCryptWorker.trafficMessage msgRequestSource = new MessageCryptWorker.trafficMessage();
            //发送类实体化
            MessageCryptWorker worker = new MessageCryptWorker();
            worker.EncodeCode = Encoding.UTF8; //encoding 使用utf8

            worker.PFXFile = pfxFilePath; //商户pfx证书路径
            worker.PFXPassword = pfxPassword;  //商户pfx证书密码
            worker.CerFile = cerFilePath; //杉德cer证书路径


            msgRequestSource.merId = merId; //商户号
            msgRequestSource.transCode = "RTPM";        //交易代码
            msgRequestSource.extend = "";               //扩展域

            //报文体json
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            msgRequestSource.encryptData = jsonSer.Serialize(dic);
            //encrytpKey会在发送前加密时自动生成16位的随机字符


            log.Write("待发送报文为：" + msgRequestSource.encryptData);
            //string ServerUrl = "http://61.129.71.103:7970/agent-main/openapi/agentpay";
            string ServerUrl = "https://caspay.sandpay.com.cn/agent-main/openapi/agentpay";

            MessageCryptWorker.trafficMessage respMessage = worker.postMessage(ServerUrl, msgRequestSource);
            log.Write("服务器返回为：" + respMessage.encryptData);

            return respMessage;
        }

    }
}
