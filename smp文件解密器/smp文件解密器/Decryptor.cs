using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmpFileDecryptor
{
    static class Decryptor
    {
        static TargetFile file;

        private static byte[] findKey(bool mod)
        {
            byte[] b1 = new byte[128];
            if (mod)
            {
                for (int i = 0; i < 128; i++) b1[i] = file.Target_file[file.Target_file.Length - 128 + i];
            }
            else
            {
                for (int i = 0; i < 128; i++) b1[i] = file.Target_file[i];
            }
            string str = ByteArrayToHexString(b1);

            for (int len = str.Length / 2; len >= 0; len--)
            {
                for (int i = 0; i < str.Length - len; i++)
                {
                    string t1 = str.Substring(i, len);
                    string t2 = str.Substring(i + len);
                    if (t2.IndexOf(t1) != -1 && t1.Length==8) return HexStringToByteArray(t1);
                }
            }
            return null;
        }

        public static void decode(TargetFile f, string save_path, string save_suffix,string key)
        {
            file = f;
            if(key=="")
            {
                file.Key = findKey(true);
                if (file.Key == null) file.Key = findKey(false);
            }
            else
            {
                file.Key = HexStringToByteArray(key);
            }
            int idx = 0;
            for (long i = 0; i < file.Target_file.Length; ++i)
                file.Target_file[i] ^= file.Key[idx++ % file.Key.Length];

            file.saveFile(save_path, save_suffix);
        }
        private static byte[] HexStringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
                hex = "0" + hex;
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < hex.Length; i += 2)
                bytes.Add(byte.Parse(hex.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
            return bytes.ToArray();
        }
        public static string ByteArrayToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

    }
}
