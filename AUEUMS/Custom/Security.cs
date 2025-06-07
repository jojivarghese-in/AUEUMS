using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;

namespace AUEUMS.Custom
{
    public class Security
    {
        
       
        public Security()
        {
      
           
        }
        public string GetPasswordEncrypt(bool useHashing = true)
        {
            
            //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-3.1
                   
            byte[] resultArray = null;
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes("asemeco@2020byITSolutionDelivery");
                if (useHashing)
                {
                    using (MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider())
                    {
                        keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes("sdasda123123123123asdasd123123123asdad1312312312v123v3v123a3&T&8dfdfdsfdsf"));
                    }
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes("sdasda123123123123asdasd123123123asdad1312312312v123v3v123a3&T&8dfdfdsfdsf");


                using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = keyArray;
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;
                    ICryptoTransform cTransform = tdes.CreateEncryptor();
                    resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                }

            }
            catch (Exception ex)
            {
                //SimpleLogger.Log(ex);
            }
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }        
    }
}
