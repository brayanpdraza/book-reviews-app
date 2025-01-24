using System;
using System.Security.Cryptography;
using Dominio.Servicios.ServicioEncripcion.Contratos;

namespace AdaptadorEncripter
{
    public class PBKDF2Encription : IEncription
    {
        public string Encriptar(string password)
        {
            byte[] hashBytes, hash;
            var salt = GenerateSalt();

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                hash = deriveBytes.GetBytes(20);
                hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);

                return Convert.ToBase64String(hashBytes);
            }

        }
        private byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);
                return salt;
            }
        }

        public bool VerificarClaveEncriptada(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = deriveBytes.GetBytes(20);
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }
            }

            return true;
        }
    }
}
