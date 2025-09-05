using System.Text;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.Security
{
    public static class RSAProviderExtentionMethods
    {
        public static string DecryptDataToString(this byte[] encryptData)
        {
            return Convert.ToBase64String(encryptData);
        }

        public static string DataToString(this byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
