using ComLibrary;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SandLibrary
{
    class SandUtil
    {
        LogUtil log = new LogUtil();
        public IDictionary<string, string> buildReqParams(string merchId, string transCode, string data)
        {


            Dictionary<string, string> formparams = new Dictionary<string, string>();
            formparams.Add("merId", merchId);
            formparams.Add("transCode", transCode);
            string encryptData ="";
            string encryptKey = "";
            string sign = "";
            genEncryptData(data, "C:/Users/lijingneng/Documents/Visual Studio 2012/Projects/OnlinePayTools/SandLibrary/SAND_PUBLIC_KEY.cer", "C:/Users/lijingneng/Documents/Visual Studio 2012/Projects/OnlinePayTools/SandLibrary/MID_RSA_PRIVATE_KEY.pfx", "123456", out encryptData, out sign, out encryptKey);
            formparams.Add("encryptData", encryptData);
            formparams.Add("encryptKey", encryptKey);
            formparams.Add("sign", sign);
            log.Write("encryptData:" + encryptData + "encryptKey:" + encryptKey + "sign:" + sign);
            return formparams;

        }
        public void parseRespParams(string responseStr)
        {

            //    string retEncryptKey = responseMap.get("encryptKey");
            //string retEncryptData = responseMap.get("encryptData");
            //string retSign = responseMap.get("sign");
            //LogUtil.Write("retEncryptKey:" + retEncryptKey);
            //LogUtil.Write("retEncryptData:" + retEncryptData);
            //LogUtil.Write("retSign:" + retSign);
            //    Dictionary<string, string> formparams = new Dictionary<string, string>();
            //    formparams.Add("merId", merchId);
            //    formparams.Add("transCode", transCode);
            //    genEncryptData(data,
            //    formparams.Add("encryptData", encryptData);
            //    formparams.Add("encryptKey", encryptKey);
            //    formparams.Add("sign", sign);
            //    LogUtil.Write("encryptData:" + encryptData);
            //    LogUtil.Write("encryptKey:" + encryptKey);
            //    LogUtil.Write("sign:" + sign);



        }

        public void genEncryptData(string data, string cerPath, string pfxPath, string password, out string encryptData, out string sign, out string encryptKey)
        {
             encryptData = "";
             encryptKey = "";
             sign = "";
            try
            {
                // 加载公私钥
                X509Certificate2 publicKeyInfo = DataCertificate.GetCertFromCerFile(cerPath);
                X509Certificate2 privateKeyInfo = DataCertificate.GetCertificateFromPfxFile(pfxPath, password);

                string platPublicKey = publicKeyInfo.PublicKey.Key.ToXmlString(false);

                string merchantPublicKey = privateKeyInfo.PublicKey.Key.ToXmlString(false);  // 公钥
                string merchantPrivateKey = privateKeyInfo.PrivateKey.ToXmlString(true);  // 私钥

                //byte[] plainBytes = Encoding.UTF8.GetBytes(data);
                //生成AESKEY
                string aesKey = ComUtils.CreateRandomString(16);
                //报文加密
                encryptData = Base64.EncodeBase64(Encoding.UTF8, AESUtil.Encrypt(data, aesKey));
                //生成签名
                sign = Base64.EncodeBase64(Encoding.UTF8, RSAUtil.RSAEncrypt(merchantPrivateKey, data));
                //AESKEY加密
                encryptKey = Base64.EncodeBase64(Encoding.UTF8, AESUtil.Encrypt(data, platPublicKey));

            }
            catch (Exception e)
            {
                log.Write(e, MsgType.Error);
            }
        }
        public void dncryptRetData(string data)
        {

        }
    }
}
