using ComLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SdjLibrary
{
    public class SdjDFRequestParamList : BaseRequestParamList
    {
        /// <summary>
        /// 定义Key为string类型，Value为object类型的一个Dictionary,key用|分隔，分别表示英文|中文|类型|是否显示
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> SetKeyValue()
        {
            Dictionary<string, object> ParamDiclist = new Dictionary<string, object>();
            ParamDiclist.Add("serviceType|业务类型|s|N", "agentBPay");
            //ParamDiclist.Add("payeeId|商家号|s|Y", "");
            ParamDiclist.Add("inputCharset|参数编码字符集|s|N", "UTF-8");
            ParamDiclist.Add("notifyUrl|服务器异步通知地址|s|N", "http://localhost/resp.htm");
            ParamDiclist.Add("interfaceVersion|接口版本|s|N", "V1.0");
            ParamDiclist.Add("signType|签名方式|s|N", "MD5");
            ParamDiclist.Add("sign|签名|s|N", "");
            ParamDiclist.Add("signature|加密数据|s|N", "");
            ParamDiclist.Add("orderNo|商户网站唯一订单号|s|Y", "df" + DateTime.Now.ToString("yyyyMMddHHmmss") + GetRandomString());
            ParamDiclist.Add("payAmount|商户订单总金额|s|Y", "");
            ParamDiclist.Add("bankCode|银行代码|s|N", "");
            ParamDiclist.Add("payeeAcc|银行卡号|s|Y", "");
            ParamDiclist.Add("payeeName|姓名|s|Y", "");
            ParamDiclist.Add("payeeBankName|收款银行名称|listDic|Y", bankCodeNameMapping);
            ParamDiclist.Add("bankAccountType|对公或对私|listDic|Y", bankAccountTyperDicList);
            return ParamDiclist;
        }
        ///// <summary>
        ///// 得到根据指定的Key行到Value
        ///// </summary>
        //public Dictionary<string, object> GetKeyValue()
        //{
        //    return SetKeyValue();
        //}

        private string GetRandomString()
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = "0123456789";
            for (int i = 0; i < 4; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
        private List<DictionaryEntry> bankAccountTyperDicList = new List<DictionaryEntry>() { new DictionaryEntry("对私", "INDIVIDUAL"), new DictionaryEntry("对公", "OPEN") };
        private List<DictionaryEntry> bankCodeNameMapping = new List<DictionaryEntry>() { new DictionaryEntry("农业银行", "ABC"), new DictionaryEntry("工商银行", "ICBC"), new DictionaryEntry("建设银行", "CCB"), new DictionaryEntry("交通银行", "BCM"), new DictionaryEntry("中国银行", "BC"), new DictionaryEntry("招商银行", "CMB"), new DictionaryEntry("民生银行", "CMBC"), new DictionaryEntry("光大银行", "EBBC"), new DictionaryEntry("兴业银行", "CIB"), new DictionaryEntry("中国邮政", "PSBC"), new DictionaryEntry("平安银行", "PAB"), new DictionaryEntry("中信银行", "CTIB"), new DictionaryEntry("广东发展银行", "GDB"), new DictionaryEntry("华夏银行", "HXB"), new DictionaryEntry("浦发银行", "SPDB"), new DictionaryEntry("东亚银行", "BEA"), new DictionaryEntry("中国移动手机支付", "CMPAY"), new DictionaryEntry("北京银行", "BOB"), new DictionaryEntry("宁波银行", "NBCB"), new DictionaryEntry("杭州银行", "HZB"), new DictionaryEntry("上海银行", "SHB"), new DictionaryEntry("包商银行", "BSB"), new DictionaryEntry("江苏银行", "JSBC"), new DictionaryEntry("南粤银行", "NYB"), new DictionaryEntry("广州银行", "GZCB"), new DictionaryEntry("浙商银行", "CZB"), new DictionaryEntry("渤海银行", "CBHB"), new DictionaryEntry("华润银行", "CRBANK"), new DictionaryEntry("兰州银行", "LZCB"), new DictionaryEntry("厦门银行", "XMB") };
    }
}