using System.Security.Cryptography;
using System.Text;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.Security
{
    public class RSAProviderKeyGenerator
    {

        public void GenerateKeys()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.KeySize = 2048;
                string publicKey = rsa.ToXmlString(false);
                SaveKeyToFile(RSATisurConst.publicKey, publicKey);
                string privateKey = rsa.ToXmlString(true);
                SaveKeyToFile(RSATisurConst.privateKey, privateKey);
            }
        }

        public void SaveKeyToFile(string fileName, string key)
        {
            //deberia ir cifrado?
            using (StreamWriter sw = new StreamWriter(fileName,false, Encoding.UTF8))
            {
                sw.Write(key);
            }
        }
        public string ReadKeyFromFile(string fileName)
        {
            string key;
            using (StreamReader sr = new StreamReader(fileName, Encoding.UTF8))
            {
                key = sr.ReadToEnd();
            }
            return key;
        }
    }

    public class RSATisurConst
    {
        public static readonly string privateKey = "privateKey.xml";
        public static readonly string publicKey  = "publicKey.xml";
    }
}
