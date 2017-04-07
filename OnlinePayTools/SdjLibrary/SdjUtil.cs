using ComLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SdjLibrary
{
    public class SdjUtil
    {
        static LogUtil log = new LogUtil();



        public static string sign(string encryptStr, string prvPath, string prvPws)
        {
            //string sign = RSAUtil.Base64Encoder(RSAUtil.CreateSignWithPrivateKey(RSAUtil.getBytesFromString(encryptStr, Encoding.UTF8),RSAUtil.getPrivateKeyXmlFromPFX(prvPath, prvPws)));//测试环境
            string sign = RSAUtil.Base64Encoder(RSAUtil.CreateSignWithPrivateKeyBySHA256(RSAUtil.getBytesFromString(encryptStr, Encoding.UTF8), RSAUtil.getPrivateKeyXmlFromPFX(prvPath, prvPws)));//生产环境
           
            return sign;
        }

        public static string createSdjDKLinkStr(Dictionary<string, string> paramDic)
        {

            string signStr = "certNo=" + paramDic["certNo"].ToString()
                + "&inputCharset=" + paramDic["inputCharset"].ToString()
                + "&interfaceVersion=" + paramDic["interfaceVersion"].ToString()
                //+ "&notifyUrl=" + paramDic["notifyUrl"].ToString()
                + "&orderNo=" + paramDic["orderNo"].ToString()
                + "&payAmount=" + paramDic["payAmount"].ToString()
                + "&payeeId=" + paramDic["payeeId"].ToString()
                + "&payerAcc=" + paramDic["payerAcc"].ToString()
                + "&platformCode=" + paramDic["platformCode"].ToString()
                + "&serviceType=" + paramDic["serviceType"].ToString();
            return signStr;
        }

        public static string createSdjDFLinkStr(Dictionary<string, string> paramDic)
        {
            string signStr = "bankAccountType=" + paramDic["bankAccountType"].ToString()
                + "&bankCode=" + paramDic["bankCode"].ToString()
                + "&inputCharset=" + paramDic["inputCharset"].ToString()
                + "&interfaceVersion=" + paramDic["interfaceVersion"].ToString()
                //+ "&notifyUrl=" + paramDic["notifyUrl"].ToString()
                + "&orderNo=" + paramDic["orderNo"].ToString()
                + "&payAmount=" + paramDic["payAmount"].ToString()
                + "&payeeAcc=" + paramDic["payeeAcc"].ToString()
                + "&payeeId=" + paramDic["payeeId"].ToString()
                + "&serviceType=" + paramDic["serviceType"].ToString();
            return signStr;
        }

        public static string createSdjQueryLinkStr(Dictionary<string, string> paramDic)
        {

            string signStr ="inputCharset=" + paramDic["inputCharset"].ToString()
                + "&interfaceVersion=" + paramDic["interfaceVersion"].ToString()
                + "&orderNo=" + paramDic["orderNo"].ToString()
                + "&payeeId=" + paramDic["payeeId"].ToString()
                + "&queryType=" + paramDic["queryType"].ToString()
                + "&serviceType=" + paramDic["serviceType"].ToString();
            return signStr;
        }

        private string sortDicToStr(Dictionary<string, string> paramDic)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>(paramDic);

            list.Sort(delegate(KeyValuePair<string, string> pair1, KeyValuePair<string, string> pair2)
            {
                return pair1.Key.CompareTo(pair2.Key);
            });

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> pair in list)
            {
                if (string.IsNullOrEmpty(pair.Value))
                {
                    sb.Append(pair.Key);
                    sb.Append("=");
                    sb.Append(pair.Value);
                    sb.Append("&");
                }
            }

            string str = sb.ToString().TrimEnd('&');
            return str;
        }

        public static string cerEncrypt(string md5SignStr, string cerPath)
        {
            string signature = RSAUtil.Base64Encoder(RSAUtil.RSAEncrypt(RSAUtil.getPublicKeyXmlFromCer(cerPath).PublicKey.Key.ToXmlString(false),
            RSAUtil.getBytesFromString(md5SignStr, Encoding.UTF8)));
            return signature;
        }
    }
}
