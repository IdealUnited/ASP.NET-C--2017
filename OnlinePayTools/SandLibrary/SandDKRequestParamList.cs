using ComLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SandLibrary
{
    public class SandDKRequestParamList : BaseRequestParamList
    {

        /// <summary>
        /// 定义Key为string类型，Value为object类型的一个Dictionary,key用|分隔，分别表示英文|中文|类型
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> SetKeyValue()
        {
            Dictionary<string, object> ParamDiclist = new Dictionary<string, object>();
            ParamDiclist.Add("version|版本号|s|N","01");
            //ParamDiclist.Add("productId|产品ID|s|N", "00000002");
            ParamDiclist.Add("tranTime|交易时间|s|Y", DateTime.Now.ToString("yyyyMMddHHmmss"));
            ParamDiclist.Add("orderCode|订单号|s|Y", "dk" + DateTime.Now.ToString("yyyyMMddHHmmss") + GetRandomString());
            ParamDiclist.Add("tranAmt|金额（必填）|s|Y", "");
            ParamDiclist.Add("currencyCode|币种|s|N","156");
            ParamDiclist.Add("accAttr|账户属性|listDic|Y", accAttrDicList);
            ParamDiclist.Add("accType|账号类型|listDic|Y", accTypeDicList);
            ParamDiclist.Add("accNo|扣款账户号|s|Y", "");
            ParamDiclist.Add("accName|账户名|s|Y", "");
            ParamDiclist.Add("bankName|账户开户行名称|s|N", "");
            ParamDiclist.Add("provNo|开户省份编码|s|N", "010000");
            ParamDiclist.Add("cityNo|开户城市编码|s|N", "");
            ParamDiclist.Add("certType|开户证件类型|listDic|Y", certTypIndivDicList);
            ParamDiclist.Add("certNo|开户证件号码|s|Y", "");
            //ParamDiclist.Add("cardId|身份证号|s|Y", "");
            ParamDiclist.Add("phone|银行预留手机号|s|Y", "");
            ParamDiclist.Add("bankInsCode|银联机构号|s|N", "");
            ParamDiclist.Add("purpose|用途说明|s|N", "collection");
            ParamDiclist.Add("reqReserved|请求方保留域|s|N", "");
            ParamDiclist.Add("extend|扩展域|s|N", "");
            return ParamDiclist;
        }

        public override string resetReqNoAndReqTime()
        {
            return "1|T|2|O";
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