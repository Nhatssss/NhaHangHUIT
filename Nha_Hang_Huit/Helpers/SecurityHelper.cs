using System.Security.Cryptography;
using System.Text;

namespace Nha_Hang_Huit.Helpers
{
    /// <summary>
    /// Helper xu ly bao mat: hash mat khau
    /// Su dung SHA256 de thay the plaintext
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Hash mat khau bang SHA256, tra ve chuoi hex lowercase
        /// </summary>
        public static string HashMatKhau(string matKhau)
        {
            if (string.IsNullOrEmpty(matKhau))
                return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(matKhau));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
