using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using ComLibrary;

namespace CryptUtils
{
    public class CryptUtils
    {
        LogUtil log = new LogUtil();
        public static byte[] AESEncrypt(byte[] Data, string Key)
        {
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(Key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(Data, 0, Data.Length);

            return resultArray;
        }

        public static byte[] AESDecrypt(byte[] Data, string Key)
        {

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(Key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(Data, 0, Data.Length);

            return resultArray;
        }


        public static string GuidTo16String()
        {
            string dic = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int keylen = dic.Length;
            long x = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                x *= ((int)b + 1);
            string value = string.Empty;
            Random ra = new Random((int)(x & 0xffffffffL) | (int)(x >> 32));
            for(int i = 0;i<16;i++)
            {
                value += dic[ra.Next() % keylen];
            }
            return value;
        }

        public static string getStringFromBytes(byte[] hexbytes, Encoding enc)
        {
            return enc.GetString(hexbytes);
        }

        public static byte[] getBytesFromString(string str, Encoding enc)
        {
            return enc.GetBytes(str);
        }

        public static byte[] asc2hex(string hexString)
        {

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static string hex2asc(byte[] hexbytes)
        {
            return BitConverter.ToString(hexbytes).Replace("-", string.Empty);

        }
        /// <summary>   
        /// RSA解密   要加密较长的数据，则可以采用分段加解密的方式
        /// </summary>   
        /// <param name="xmlPrivateKey"></param>   
        /// <param name="EncryptedBytes"></param>   
        /// <returns></returns>   
        public static byte[] RSADecrypt(string xmlPrivateKey, byte[] EncryptedBytes)
        {

            using (RSACryptoServiceProvider RSACryptography = new RSACryptoServiceProvider())
            {
                RSACryptography.FromXmlString(xmlPrivateKey);


                int MaxBlockSize = RSACryptography.KeySize / 8;    //解密块最大长度限制

                if (EncryptedBytes.Length <= MaxBlockSize)
                    return RSACryptography.Decrypt(EncryptedBytes, false);

                using (MemoryStream CrypStream = new MemoryStream(EncryptedBytes))
                using (MemoryStream PlaiStream = new MemoryStream())
                {
                    Byte[] Buffer = new Byte[MaxBlockSize];
                    int BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);

                    while (BlockSize > 0)
                    {
                        Byte[] ToDecrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToDecrypt, 0, BlockSize);

                        Byte[] Plaintext = RSACryptography.Decrypt(ToDecrypt, false);
                        PlaiStream.Write(Plaintext, 0, Plaintext.Length);

                        BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
                    }

                    return PlaiStream.ToArray();
                }
            }
        }

        //public static byte[] RSADecrypt(string xmlPrivateKey, byte[] EncryptedBytes)
        //{
        //    RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
        //    provider.FromXmlString(xmlPrivateKey);
        //    return provider.Decrypt(EncryptedBytes, false);
        //}

        /// <summary>   
        /// RSA加密   要加密较长的数据，则可以采用分段加解密的方式
        /// </summary>   
        /// <param name="xmlPublicKey"></param>   
        /// <param name="SourceBytes"></param>   
        /// <returns></returns>   
        public static byte[] RSAEncrypt(string xmlPublicKey, byte[] SourceBytes)
        {
            using (RSACryptoServiceProvider RSACryptography = new RSACryptoServiceProvider())
            {
                RSACryptography.FromXmlString(xmlPublicKey);

                int MaxBlockSize = RSACryptography.KeySize / 8 - 11;    //加密块最大长度限制

                if (SourceBytes.Length <= MaxBlockSize)
                    return RSACryptography.Encrypt(SourceBytes, false);

                using (MemoryStream PlaiStream = new MemoryStream(SourceBytes))
                using (MemoryStream CrypStream = new MemoryStream())
                {
                    Byte[] Buffer = new Byte[MaxBlockSize];
                    int BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);

                    while (BlockSize > 0)
                    {
                        Byte[] ToEncrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                        Byte[] Cryptograph = RSACryptography.Encrypt(ToEncrypt, false);
                        CrypStream.Write(Cryptograph, 0, Cryptograph.Length);

                        BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                    }

                    return CrypStream.ToArray();
                }
            }
        }

        //public static byte[] RSAEncrypt(string xmlPublicKey, byte[] SourceBytes)
        //{
        //    RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
        //    provider.FromXmlString(xmlPublicKey);
        //    return provider.Encrypt(SourceBytes, false);
        //}

        public static X509Certificate2 getPrivateKeyXmlFromPFX(string PFX_file, string password)
        {
            return new X509Certificate2(PFX_file, password, X509KeyStorageFlags.Exportable);
        }

        public static X509Certificate2 getPublicKeyXmlFromCer(string Cer_file)
        {
            return new X509Certificate2(Cer_file);
        }

        public static byte[] CreateSignWithPrivateKey(byte[] msgin, X509Certificate2 pfx)
        {
            HashAlgorithm SHA1 = HashAlgorithm.Create("sha1");
            byte[] hashdata = SHA1.ComputeHash(msgin);//求数字指纹

            RSAPKCS1SignatureFormatter signCrt = new RSAPKCS1SignatureFormatter();
            signCrt.SetKey(pfx.PrivateKey);
            signCrt.SetHashAlgorithm("sha1");
            return signCrt.CreateSignature(hashdata);

        }

        public static bool VerifySignWithPublicKey(byte[] msgin, X509Certificate2 cer, byte[] sign)
        {
            HashAlgorithm SHA1 = HashAlgorithm.Create("sha1");
            byte[] hashdata = SHA1.ComputeHash(msgin);//求数字指纹
            SHA1Managed hash = new SHA1Managed();
            byte[] hashedData;

            RSACryptoServiceProvider signV = new RSACryptoServiceProvider();
            signV.FromXmlString(cer.PublicKey.Key.ToXmlString(false));
            //return signV.VerifyData(hashdata, "sha1", sign);

            return signV.VerifyData(msgin, CryptoConfig.MapNameToOID("SHA1"), sign);
        }

        public static string Base64Encoder(byte[] b)
        {
            return Convert.ToBase64String(b);
        }

        public static byte[] Base64Decoder(string base64String)
        {
            return Convert.FromBase64String(base64String);
        }

    }

    public class MessageCryptWorker
    {
        LogUtil log = new LogUtil();
        public struct trafficMessage
        {
            public string transCode;//交易码 根据具体业务定义
            public string merId;  //合作商户ID 杉德系统分配，唯一标识
            public string encryptKey;// 加密后的AES秘钥 公钥加密(RSA/ECB/PKCS1Padding)，加密结果采用base64编码
            public string encryptData; //加密后的请求/应答报文 AES加密(AES/ECB/PKCS5Padding)，加密结果采用base64编码
            public string sign;//    签名 对encryptData对应的明文进行签名(SHA1WithRSA)，签名结果采用base64编码
            public string extend;//  扩展域 暂时不用

        }

        private Encoding encodeCode = Encoding.UTF8;
        public Encoding EncodeCode
        {
            get
            {
                return encodeCode;
            }
            set
            {
                encodeCode = value;
            }
        }


        private string pfxFilePath = string.Empty;
        public string PFXFile
        {
            get
            {
                return pfxFilePath;
            }
            set
            {
                pfxFilePath = value;
            }
        }

        private string pfxPassword = string.Empty;
        public string PFXPassword
        {
            get
            {
                return pfxPassword;
            }
            set
            {
                pfxPassword = value;
            }
        }


        private string cerFilePath = string.Empty;
        public string CerFile
        {
            get
            {
                return cerFilePath;
            }
            set
            {
                cerFilePath = value;
            }
        }

        private trafficMessage EncryptMessageBeforePost(trafficMessage msgSource)
        {
            trafficMessage msgEncrypt = new trafficMessage();
            //随机生成16位密钥
            msgSource.encryptKey = CryptUtils.GuidTo16String();
            log.Write("Generated local AESkey [" + msgSource.encryptKey + "]");

            //encryptKey加密，杉德公钥RSA加密
            msgEncrypt.encryptKey = CryptUtils.Base64Encoder(CryptUtils.RSAEncrypt(CryptUtils.getPublicKeyXmlFromCer(cerFilePath).PublicKey.Key.ToXmlString(false),
                CryptUtils.getBytesFromString(msgSource.encryptKey, encodeCode)));

            //log.Write("encryptKey[" + msgSource.encryptKey + "][" + msgEncrypt.encryptKey + "]");

            msgEncrypt.transCode = msgSource.transCode;
            msgEncrypt.merId = msgSource.merId;
            msgEncrypt.extend = msgSource.extend;

            //encryptData加密，AES加密
            msgEncrypt.encryptData = CryptUtils.Base64Encoder(CryptUtils.AESEncrypt(CryptUtils.getBytesFromString(msgSource.encryptData, encodeCode),
                msgSource.encryptKey));

            //log.Write("encryptData[" + msgSource.encryptData + "][" + msgEncrypt.encryptData + "]");

            msgEncrypt.sign = CryptUtils.Base64Encoder(CryptUtils.CreateSignWithPrivateKey(CryptUtils.getBytesFromString(msgSource.encryptData, encodeCode),
                CryptUtils.getPrivateKeyXmlFromPFX(pfxFilePath, pfxPassword)));

            //log.Write("sign[" + msgEncrypt.sign + "]");

            return msgEncrypt;

        }
        private trafficMessage DecryptMessageAfterResponse(trafficMessage msgEncrypt)
        {
            trafficMessage msgSource = new trafficMessage();
            msgSource.transCode = msgEncrypt.transCode;
            msgSource.merId = msgEncrypt.merId;
            msgSource.extend = msgEncrypt.extend;
            msgSource.encryptKey = CryptUtils.getStringFromBytes(CryptUtils.RSADecrypt(CryptUtils.getPrivateKeyXmlFromPFX(pfxFilePath, pfxPassword).PrivateKey.ToXmlString(true),
                CryptUtils.Base64Decoder(msgEncrypt.encryptKey)), encodeCode);
            log.Write("Decrypted remote AESkey [" + msgSource.encryptKey + "]");
            //log.Write("encryptKey[" + msgSource.encryptKey + "]);

            byte[] sourceByte = CryptUtils.AESDecrypt(CryptUtils.Base64Decoder(msgEncrypt.encryptData),
                msgSource.encryptKey);
            msgSource.encryptData = CryptUtils.getStringFromBytes(sourceByte, encodeCode);
           // log.Write("encryptData[" + msgSource.encryptData + "][" + msgEncrypt.encryptData + "]");

            msgSource.sign =
                CryptUtils.VerifySignWithPublicKey(
                    sourceByte,
                    CryptUtils.getPublicKeyXmlFromCer(cerFilePath),
                    CryptUtils.Base64Decoder(msgEncrypt.sign)
                    ).ToString();

           // log.Write("sign[" + msgSource.sign + "][" + msgEncrypt.sign + "]");
            //需要添加引用 System.Web
            return msgSource;
        }

        private trafficMessage UrlDecodeMessage(string msgResponse)
        {
            trafficMessage msgEncrypt = new trafficMessage();
            string[] EncryptBody = msgResponse.Split('&');
            for (int i = 0; i < EncryptBody.Length; i++)
            {
                string[] tmp = EncryptBody[i].Split('=');
                switch (tmp[0])
                {
                    //需要添加引用System.Web，用于url转码，处理base64产生的+/=
                    case "transCode": msgEncrypt.transCode = System.Web.HttpUtility.UrlDecode(EncryptBody[i].Replace("transCode=", "").Trim('"')); break;
                    case "merId": msgEncrypt.merId = System.Web.HttpUtility.UrlDecode(EncryptBody[i].Replace("merId=", "").Trim('"')); break;
                    case "encryptKey": msgEncrypt.encryptKey = System.Web.HttpUtility.UrlDecode(EncryptBody[i].Replace("encryptKey=", "").Trim('"')); break;
                    case "encryptData": msgEncrypt.encryptData = System.Web.HttpUtility.UrlDecode(EncryptBody[i].Replace("encryptData=", "").Trim('"')); break;
                    case "sign": msgEncrypt.sign = System.Web.HttpUtility.UrlDecode(EncryptBody[i].Replace("sign=", "").Trim('"')); break;
                    case "extend": msgEncrypt.extend = System.Web.HttpUtility.UrlDecode(EncryptBody[i].Replace("extend=", "").Trim('"')); break;
                }
            }
            return msgEncrypt;
        }
        private string UrlEncodeMessage(trafficMessage msgRequest)
        {
            //需要添加引用System.Web，用于url转码，处理base64产生的+/=
            return "transCode=" + System.Web.HttpUtility.UrlEncode(msgRequest.transCode) + "&" +
                 "merId=" + System.Web.HttpUtility.UrlEncode(msgRequest.merId) + "&" +
                  "encryptKey=" + System.Web.HttpUtility.UrlEncode(msgRequest.encryptKey) + "&" +
                   "encryptData=" + System.Web.HttpUtility.UrlEncode(msgRequest.encryptData) + "&" +
                   "sign=" + System.Web.HttpUtility.UrlEncode(msgRequest.sign) + "&" +
                   "extend=" + System.Web.HttpUtility.UrlEncode(msgRequest.extend);
        }

        public trafficMessage postMessage(string serverUrl, trafficMessage requestSourceMessage)
        {
            trafficMessage responseMessage = new trafficMessage();
            try
            {
                string requestString = UrlEncodeMessage(EncryptMessageBeforePost(requestSourceMessage));
                //log.Write("url:" + serverUrl);
                log.Write("request  ==>[" + requestString + "]");

                string responseString = HttpUtils.HttpPost(serverUrl, requestString, encodeCode);
                log.Write("response <==[" + responseString + "]");
                responseMessage = DecryptMessageAfterResponse(UrlDecodeMessage(responseString));
            }
            catch(Exception er)
            {
                log.Write(er.ToString());
            }
            return responseMessage;
        }
    }

    public class HttpUtils
    {
        LogUtil log = new LogUtil();
        public static string HttpPost(string postUrl, string paramData, Encoding EncodingName)
        {

            string postDataStr = paramData;
            byte[] buff = EncodingName.GetBytes(postDataStr);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(buff, 0, buff.Length);
            myRequestStream.Close();


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, EncodingName);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
    }

}
