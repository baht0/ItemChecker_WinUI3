using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ItemChecker.Net.Session
{
    internal class RsaPassword : HttpRequest
    {
        public string Mod { get; set; }
        public string Exp { get; set; }
        public string TimeStamp { get; set; }
        public string EncryptedPassword { get; set; }

        internal async Task<RsaPassword> GetEncryptedPasswordAsync(string accountName, string password)
        {
            await GetPasswordRSAPublicKeyAsync(accountName);

            byte[] encryptedPasswordBytes;
            using (var rsaEncryptor = new RSACryptoServiceProvider())
            {
                var passwordBytes = Encoding.ASCII.GetBytes(password);
                var rsaParameters = rsaEncryptor.ExportParameters(false);
                rsaParameters.Modulus = Convert.FromHexString(Mod);
                rsaParameters.Exponent = Convert.FromHexString(Exp);
                rsaEncryptor.ImportParameters(rsaParameters);
                encryptedPasswordBytes = rsaEncryptor.Encrypt(passwordBytes, false);
            }
            EncryptedPassword = Convert.ToBase64String(encryptedPasswordBytes);

            return this;
        }
        async Task GetPasswordRSAPublicKeyAsync(string accountName)
        {
            string response = await RequestGetAsync("https://api.steampowered.com/IAuthenticationService/GetPasswordRSAPublicKey/v1/?account_name=" + accountName);
            JObject json = JObject.Parse(response);

            Mod = json["response"]["publickey_mod"].ToString();
            Exp = json["response"]["publickey_exp"].ToString();
            TimeStamp = json["response"]["timestamp"].ToString();
        }
    }
}