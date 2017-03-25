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
            string sign = "";

            X509Certificate2 privateKeyInfo = DataCertificate.GetCertificateFromPfxFile(prvPath, prvPws);
            string merchantPublicKey = privateKeyInfo.PublicKey.Key.ToXmlString(false);  // 公钥
            string merchantPrivateKey = privateKeyInfo.PrivateKey.ToXmlString(true);  // 私钥
            sign = RSAUtil.RSAEncrypt(merchantPrivateKey, encryptStr);
            return sign;
        }

        public static string generateMd5Sign(Dictionary<string, string> paramDic, string orgKey)
        {
            string str = createSdjLinkStr(paramDic);
            //MD5加密
            string signStr = str + "&merKey=" + orgKey;
            log.Write("盛迪嘉待加签德字符串:" + signStr);

            string sign = Md5Util.encrypt(signStr);

            return sign;
            //str 值key1=value1&key2=value2&key3=value3&key4=value4
        }

        public static string createSdjLinkStr(Dictionary<string, string> paramDic)
        {

            string signStr = "certNo=" + paramDic["certNo"].ToString()
                + "&inputCharset=" + paramDic["inputCharset"].ToString()
                + "&interfaceVersion=" + paramDic["interfaceVersion"].ToString()
                + "&notifyUrl=" + paramDic["notifyUrl"].ToString()
                + "&orderNo=" + paramDic["orderNo"].ToString()
                + "&payeeId=" + paramDic["payeeId"].ToString()
                + "&payerAcc=" + paramDic["payerAcc"].ToString()
                + "&platformCode=" + paramDic["platformCode"].ToString()
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
            string cerEncryptStr = "";
            // 加载公私钥
            X509Certificate2 publicKeyInfo = DataCertificate.GetCertFromCerFile(cerPath);
            string platPublicKey = publicKeyInfo.PublicKey.Key.ToXmlString(false);
            cerEncryptStr = RSAUtil.RSAEncrypt(platPublicKey, md5SignStr);
            return cerEncryptStr;
        }


        /// <summary>
        /// 使用私钥加密字符串
        /// </summary>
        /// <param name="key">需加密的字符</param>
        /// <param name="keyPath">私钥证书文件地址</param>
        public string EncryptKey(string key, string keyPath)
        {
            X509Certificate2 c2 = new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + "bin\\client.cer");

            string keyPublic2 = c2.PublicKey.Key.ToXmlString(false);

            string cypher2 = RSAEncrypt(keyPublic2, key);  // 加密  
            return cypher2;
        }


        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="m_strDecryptString"></param>
        /// <returns></returns>
        static string RSADecrypt(string xmlPrivateKey, string m_strDecryptString)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPrivateKey);
            byte[] rgb = Convert.FromBase64String(m_strDecryptString);
            byte[] bytes = provider.Decrypt(rgb, false);
            return new UnicodeEncoding().GetString(bytes);
        }
        /// <summary>   
        /// RSA加密   
        /// </summary>   
        /// <param name="xmlPublicKey"></param>   
        /// <param name="m_strEncryptString"></param>   
        /// <returns></returns>   
        static string RSAEncrypt(string xmlPublicKey, string m_strEncryptString)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPublicKey);
            byte[] bytes = new UnicodeEncoding().GetBytes(m_strEncryptString);
            return Convert.ToBase64String(provider.Encrypt(bytes, false));
        }


    }
}
