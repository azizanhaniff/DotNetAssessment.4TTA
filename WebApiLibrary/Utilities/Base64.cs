using System.Text;

namespace WebApiLibrary.Utilities
{
    public static class Base64
    {
        public static string Encode(string input)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytesToEncode);
        }

        public static string Decode(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
