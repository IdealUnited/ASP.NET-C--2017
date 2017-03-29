using System.Web.Security;
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
            string md5EncryptStr = FormsAuthentication.HashPasswordForStoringInConfigFile(strText, "MD5").ToLower() ;
            return md5EncryptStr;
        }
    }
}
