using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ComLibrary
{
    public class Md5Util
    {
        ///   <summary>
        ///   给一个字符串进行MD5加密
        ///   </summary>
        ///   <param   name="strText">待加密字符串</param>
        ///   <returns>加密后的字符串</returns>
        public static string encrypt(string strText)
        {
            //MD5 md5 = new MD5CryptoServiceProvider();
            //byte[] result = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strText));
            //return System.Text.Encoding.UTF8.GetString(result);
           
            //获取加密服务
            System.Security.Cryptography.MD5CryptoServiceProvider md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider();
       
            //获取要加密的字段，并转化为Byte[]数组
            byte[] testEncrypt = System.Text.Encoding.Unicode.GetBytes(strText);

            //加密Byte[]数组
            byte[] resultEncrypt = md5CSP.ComputeHash(testEncrypt);

            //将加密后的数组转化为字段(普通加密)
            string encriptStr = System.Text.Encoding.Unicode.GetString(resultEncrypt);
            return encriptStr;
        }
    }

}
