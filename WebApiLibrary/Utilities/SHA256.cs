using System.Text;

namespace WebApiLibrary.Utilities
{
    public static class SHA256
    {
        public static string Encode(string input)
        {
            using System.Security.Cryptography.SHA256 sha256Hash = System.Security.Cryptography.SHA256.Create();

            byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            var builder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
