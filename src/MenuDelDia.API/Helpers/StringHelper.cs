using System;
using System.IO;
using System.Linq;

namespace MenuDelDia.API.Helpers
{
    public static class StringHelper
    {
        public static string GenerateRandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

        }


        public static string EncodeToBase64(string path)
        {
            FileStream inFile;
            byte[] binaryData;

            try
            {
                inFile = new FileStream(path, FileMode.Open, FileAccess.Read);
                binaryData = new Byte[inFile.Length];
                long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);
                inFile.Close();
            }
            catch
            {
                // Error creating stream or reading from it.
                return string.Empty;
            }

            // Convert the binary input into Base64 UUEncoded output. 
            var base64String = string.Empty;
            try
            {
                base64String = Convert.ToBase64String(binaryData, 0, binaryData.Length);
            }
            catch
            {
                return string.Empty;
            }

            return base64String;
        }

    }
}