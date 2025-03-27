using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BucketProject.DAL.Data.InterfacesRepo;

namespace BucketProject.DAL.Data.Repositories
{
    public class PasswordHasher: IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32; 
        private const int Iterations = 100000;

        public (string hash, string salt) HashPassword(string password)
        {
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(KeySize);
                return (
                    Convert.ToBase64String(hashBytes),
                    Convert.ToBase64String(saltBytes)
                );
            }
        }

        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(KeySize);
                string computedHash = Convert.ToBase64String(hashBytes);
                return storedHash == computedHash;
            }
        }
    }
}
