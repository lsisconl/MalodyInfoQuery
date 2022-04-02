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
                //这个是很常见的错误，你字节转换成字符串的时候要保证是2位宽度啊，某个字节为0转换成字符串的时候必须是00的，否则就会丢失位数啊。不仅是0，1～9也一样。
                //byte2String += targetData[i].ToString("x");//这个会丢失
                byte2String = byte2String+ targetData[i].ToString("x2");
            }
            return byte2String;
        }
    }
}