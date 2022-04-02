using System.Security.Cryptography;

namespace MalodyInfoQuery.Utils
{
    internal class MD5
    {
        public static string GetMD5(string myString)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(myString);//
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String = byte2String+ targetData[i].ToString("x2");
            }
            return byte2String;
        }
    }
}