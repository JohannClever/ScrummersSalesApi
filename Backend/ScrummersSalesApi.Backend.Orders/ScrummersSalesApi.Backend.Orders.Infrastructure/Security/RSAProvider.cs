using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.Security
{
    public class RSAProvider
    {
        public byte[] EncryptData(byte[] data, string publicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), publicKey)));
                return rsa.Encrypt(data, true);
            }
        }

        // Método para descifrar datos utilizando la clave privada
        public byte[] DecryptData(byte[] data, string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), privateKey)));
                return rsa.Decrypt(data, true);
            }
        }
    }
}
