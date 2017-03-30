using ComLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SdjLibrary
{
    public class SdjDKRequestParamList : BaseRequestParamList
    {
        /// <summary>
        /// 定义Key为string类型，Value为object类型的一个Dictionary,key用|分隔，分别表示英文|中文|类型|是否显示
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> SetKeyValue()
        {
            Dictionary<string, object> ParamDiclist = new Dictionary<string, object>();
            ParamDiclist.Add("serviceType|业务类型|s|N", "agentPay");
            //ParamDiclist.Add("payeeId|商家号|s|Y", "");
            ParamDiclist.Add("inputCharset|参数编码字符集|s|N", "UTF-8");
            ParamDiclist.Add("notifyUrl|服务器异步通知地址|s|N", "http://localhost/resp.htm");
            ParamDiclist.Add("clientIp|客户端IP|s|N", "");
            ParamDiclist.Add("interfaceVersion|接口版本|s|N", "V1.0");
            ParamDiclist.Add("signType|签名方式|s|N", "MD5");
            ParamDiclist.Add("sign|签名|s|N", "");
            ParamDiclist.Add("signature|加密数据|s|N", "");
            ParamDiclist.Add("orderNo|商户网站唯一订单号|s|Y", "dk" + DateTime.Now.ToString("yyyyMMddHHmmss") + GetRandomString());
            ParamDiclist.Add("orderTime|商户订单时间|s|Y", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            ParamDiclist.Add("payAmount|商户订单总金额|s|Y", "");
            ParamDiclist.Add("platformCode|商户平台编号|s|N", "newp2p");
            ParamDiclist.Add("bankCode|银行代码|s|N", "");
            ParamDiclist.Add("payerAcc|银行卡号|s|Y", "");
            ParamDiclist.Add("payerName|姓名|s|Y", "");
            ParamDiclist.Add("provNo|行政区域编码|s|N", "");
            ParamDiclist.Add("certNo|身份证|s|Y", "");
            ParamDiclist.Add("productName|商品名称|s|N", "");
            ParamDiclist.Add("showUrl|商品展示URL|s|N", "");
            ParamDiclist.Add("productCode|商品编号|s|N", "");
            ParamDiclist.Add("productNum|商品数量|s|N", "");
            ParamDiclist.Add("productDesc|商品描述|s|N", "");
            ParamDiclist.Add("extendParam|公用业务扩展参数|s|N", "");//参数格式：参数名1^参数值|参数名2^参数值2，多条数据间用"|"间隔
            /*extendParam|公用业务扩展参数
            参数	       参数名称 	    类型（长度）	使用	说明
            收货人信息	   			        
            shipToName	   收货人姓名	    String(50)	    可选	
            shipToEmail	   收货人邮箱	    String(60)	    可选	
            shipToPhone	   收货人电话	    String(30)	    可选	
            shipToState	   收货地址省	    String	        可选	
            shipToCity	   收货地址城市	    String	        可选	
            shipToStreet   收货人详细地址	String	        可选	
            shipToZip	   收货地址邮编	    String	        可选	
            */                                            

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
        private List<DictionaryEntry> accAttrDicList = new List<DictionaryEntry>() { new DictionaryEntry("对私", "0"), new DictionaryEntry("对公", "1") };
        private List<DictionaryEntry> accTypeDicList = new List<DictionaryEntry>() { new DictionaryEntry("银行卡", "4"), new DictionaryEntry("公司账户", "3") };
        private List<DictionaryEntry> certTypIndivDicList = new List<DictionaryEntry>() { new DictionaryEntry("身份证", "0101"), new DictionaryEntry("临时身份证", "0102"), new DictionaryEntry("户口簿", "0103"), new DictionaryEntry("军官证", "0104"), new DictionaryEntry("警官证", "0105"), new DictionaryEntry("士兵证", "0106"), new DictionaryEntry("文职干部证", "0107"), new DictionaryEntry("外国护照", "0108"), new DictionaryEntry("香港通行证", "0109"), new DictionaryEntry("澳门通行证", "0110"), new DictionaryEntry("台湾通行证或有效旅行证件", "0111"), new DictionaryEntry("军官退休证", "0112"), new DictionaryEntry("中国护照", "0113"), new DictionaryEntry("外国人永久居留证", "0114"), new DictionaryEntry("军事学员证", "0115"), new DictionaryEntry("离休干部荣誉证", "0116"), new DictionaryEntry("边民出入境通行证", "0117"), new DictionaryEntry("村民委员会证明", "0118"), new DictionaryEntry("学生证", "0119"), new DictionaryEntry("护照", "0120"), new DictionaryEntry("香港居民来往内地通行证", "0121"), new DictionaryEntry("澳门居民来往内地通行证", "0122"), new DictionaryEntry("台湾同胞来往内地通行证", "0123"), new DictionaryEntry("其它", "0124") };
        private List<DictionaryEntry> certTypeCompDicList = new List<DictionaryEntry>() { new DictionaryEntry("统一社会信用代码", "0201"), new DictionaryEntry("组织机构代码证号", "0202"), new DictionaryEntry("营业执照号码", "0203"), new DictionaryEntry("登记证书", "0204"), new DictionaryEntry("国税登记证号码", "0205"), new DictionaryEntry("地税登记证号码", "0206"), new DictionaryEntry("开户许可证", "0207"), new DictionaryEntry("事业单位编号", "0208"), new DictionaryEntry("金融许可证编号", "0209"), new DictionaryEntry("其他证件", "0210") };
    }
}