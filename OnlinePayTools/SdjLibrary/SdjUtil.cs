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
            //string sign = "";

            //X509Certificate2 privateKeyInfo = DataCertificate.GetCertificateFromPfxFile(prvPath, prvPws);
            //string merchantPublicKey = privateKeyInfo.PublicKey.Key.ToXmlString(false);  // 公钥
            //string merchantPrivateKey = privateKeyInfo.PrivateKey.ToXmlString(true);  // 私钥
            //sign = RSAUtil.RSAEncrypt(merchantPublicKey, encryptStr);


            string sign = RSAUtil.Base64Encoder(RSAUtil.CreateSignWithPrivateKey(RSAUtil.getBytesFromString(encryptStr, Encoding.UTF8),RSAUtil.getPrivateKeyXmlFromPFX(prvPath, prvPws)));
            //string sign= RSAUtil.Base64Encoder(RSAUtil.CreateSignWithPrivateKey(RSAUtil.getBytesFromString("abcdefg123456", Encoding.UTF8),RSAUtil.getPrivateKeyXmlFromPFX(prvPath, prvPws)));
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

        public static string createSdjQueryLinkStr(Dictionary<string, string> paramDic)
        {

            string signStr ="&inputCharset=" + paramDic["inputCharset"].ToString()
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
            //string cerEncryptStr = "";
            //// 加载公私钥
            //X509Certificate2 publicKeyInfo = DataCertificate.GetCertFromCerFile(cerPath);
            //string platPublicKey = publicKeyInfo.PublicKey.Key.ToXmlString(false);
            //cerEncryptStr = RSAUtil.RSAEncrypt(platPublicKey, md5SignStr);
            //return cerEncryptStr;

            string signature = RSAUtil.Base64Encoder(RSAUtil.RSAEncrypt(RSAUtil.getPublicKeyXmlFromCer(cerPath).PublicKey.Key.ToXmlString(false),
            RSAUtil.getBytesFromString(md5SignStr, Encoding.UTF8)));

            //string encryptKey = RSAUtil.Base64Encoder(RSAUtil.RSAEncrypt(RSAUtil.getPublicKeyXmlFromCer(cerPath).PublicKey.Key.ToXmlString(false),
            //   RSAUtil.getBytesFromString("abcdefg123456", Encoding.UTF8)));
            //signature = "2/L4Kashpqvxv6g23RhRpQPKR6oEqu0u3iqvFcmEgH/6jpwErTz0R6uQxyB7ELW/IxmqYj4d+MFu//HCA25AzbJa1MbKxEdCU00a1C351LIcFnUMgPM4Ijs3xG2F+2efUMxWgKzTUPl2WKYnwdxiHRMXQStIgL8k6VDz9QM/kYh7e2J0JHM/JGnu0XZ3OZ2GqmYOvxUA3h4FheOmZ1q3WhySzgrVPpO7Xs7HzjfD/F2k8x+TJrocRq/5LIb2toW3i6QG0uFH60Rc2xGa9M+8HxShU5fgepdwGymeRuUCl2Av2O27wQtDnytYviw68EvE4ZlzZVx3KBF0V8A7dz6Q4A==";
            return signature;
        }
    }
}
