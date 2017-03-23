using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ComLibrary
{
    public static class ComUtils
    {
        /// <summary>  
        ///   
        /// 将对象属性转换为key-value对  
        /// </summary>  
        /// <param name="o"></param>  
        /// <returns></returns>  
        public static Dictionary<String, Object> ToDictionay(Object o)
        {
            Dictionary<String, Object> tDic = new Dictionary<string, object>();

            Type t = o.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic)
                {
                    tDic.Add(p.Name, mi.Invoke(o, new Object[] { }));
                }
            }

            return tDic;

        }

        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <param name="Letter">字母个数</param>
        /// <param name="length">数字个数</param>
        /// <returns></returns>
        public static string CreateRandomString(int length)
        {
            string str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(str[new Random(Guid.NewGuid().GetHashCode()).Next(0, str.Length - 1)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 创建字母随机数
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public static string CreateRandomLetter(int letter)
        {
            string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < letter; i++)
            {
                sb.Append(str[new Random(Guid.NewGuid().GetHashCode()).Next(0, str.Length - 1)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 创建数字随机数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string CreateRandomNum(int num)
        {
            string str = "0123456789";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < num; i++)
            {
                sb.Append(str[new Random(Guid.NewGuid().GetHashCode()).Next(0, str.Length - 1)]);
            }
            return sb.ToString();
        }

    }
}
