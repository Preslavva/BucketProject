
using System.Security.Cryptography;
using BucketProject.BLL.Business_Logic.InterfacesService;

namespace BucketProject.DAL.Data.Repositories
{
    public class PasswordHasher: IPasswordHasher
    {
        private const int saltSize = 16;
        private const int keySize = 32; 
        private const int iterations = 100000;

        public (string hash, string salt) HashPassword(string password)
        {
            byte[] saltBytes = new byte[saltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(keySize);
                return (
                    Convert.ToBase64String(hashBytes),
                    Convert.ToBase64String(saltBytes)
                );
            }
        }

        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(keySize);
                string computedHash = Convert.ToBase64String(hashBytes);
                return storedHash == computedHash;
            }
        }
    }
}
