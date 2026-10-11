using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Software
{
    /// <summary>
    /// Clase de utilidad para funciones de seguridad y cifrado
    /// Implementa PBKDF2 para hash de contraseñas (más seguro que SHA256 simple)
    /// </summary>
    public static class SecurityHelper
    {
        // Constantes para PBKDF2
        private const int SaltSize = 32; // 256 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 100000; // Número de iteraciones (alto = más seguro pero más lento)

        /// <summary>
        /// Genera un hash seguro de contraseña usando PBKDF2 con salt aleatorio
        /// Formato: salt:hash (ambos en base64)
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));

            // Generar salt aleatorio único para cada contraseña
            byte[] salt = new byte[SaltSize];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Generar hash usando PBKDF2
            byte[] hash = GeneratePBKDF2Hash(password, salt, Iterations);

            // Combinar salt y hash en formato: salt:hash (ambos en base64)
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verifica si una contraseña coincide con un hash almacenado
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            try
            {
                // Separar salt y hash del formato almacenado
                string[] parts = storedHash.Split(':');
                if (parts.Length != 2)
                {
                    // Formato antiguo (SHA256 sin salt) - intentar verificación legacy
                    return VerifyLegacyHash(password, storedHash);
                }

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] hash = Convert.FromBase64String(parts[1]);

                // Generar hash de la contraseña ingresada con el mismo salt
                byte[] testHash = GeneratePBKDF2Hash(password, salt, Iterations);

                // Comparación segura contra timing attacks
                return ConstantTimeEquals(hash, testHash);
            }
            catch
            {
                // Si hay error en el formato, intentar verificación legacy
                return VerifyLegacyHash(password, storedHash);
            }
        }

        /// <summary>
        /// Genera hash PBKDF2
        /// </summary>
        private static byte[] GeneratePBKDF2Hash(string password, byte[] salt, int iterations)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        /// <summary>
        /// Comparación segura contra timing attacks
        /// </summary>
        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }

        /// <summary>
        /// Verificación legacy para hashes SHA256 antiguos (sin salt)
        /// Permite migración gradual
        /// </summary>
        private static bool VerifyLegacyHash(string password, string storedHash)
        {
            string computedHash = GenerateLegacySHA256Hash(password);
            return string.Equals(computedHash, storedHash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Genera hash SHA256 legacy (para compatibilidad con registros antiguos)
        /// </summary>
        private static string GenerateLegacySHA256Hash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Sanitiza entrada de usuario para prevenir XSS e inyección SQL
        /// </summary>
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Eliminar caracteres peligrosos
            return HttpUtility.HtmlEncode(input.Trim());
        }

        /// <summary>
        /// Valida formato de correo electrónico
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida fortaleza de contraseña
        /// </summary>
        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsLower(c)) hasLower = true;
                if (char.IsDigit(c)) hasDigit = true;
                if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }

            // Requiere al menos 3 de 4 criterios
            int criteria = (hasUpper ? 1 : 0) + (hasLower ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSpecial ? 1 : 0);
            return criteria >= 3;
        }

        /// <summary>
        /// Registra intentos de login fallidos para protección contra fuerza bruta
        /// </summary>
        public static void RecordFailedLoginAttempt(string username, HttpContext context)
        {
            string key = $"FailedLogin_{username}";
            int attempts = 0;

            if (context.Session[key] != null)
            {
                attempts = (int)context.Session[key];
            }

            attempts++;
            context.Session[key] = attempts;
            context.Session[$"FailedLoginTime_{username}"] = DateTime.Now;
        }

        /// <summary>
        /// Verifica si un usuario está bloqueado por demasiados intentos fallidos
        /// </summary>
        public static bool IsAccountLocked(string username, HttpContext context, int maxAttempts = 5, int lockoutMinutes = 15)
        {
            string key = $"FailedLogin_{username}";
            string timeKey = $"FailedLoginTime_{username}";

            if (context.Session[key] == null)
                return false;

            int attempts = (int)context.Session[key];

            if (attempts >= maxAttempts)
            {
                if (context.Session[timeKey] != null)
                {
                    DateTime lockoutTime = (DateTime)context.Session[timeKey];
                    TimeSpan elapsed = DateTime.Now - lockoutTime;

                    if (elapsed.TotalMinutes < lockoutMinutes)
                    {
                        return true; // Aún bloqueado
                    }
                    else
                    {
                        // Tiempo de bloqueo expirado, resetear intentos
                        context.Session[key] = null;
                        context.Session[timeKey] = null;
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Obtiene minutos restantes de bloqueo
        /// </summary>
        public static int GetRemainingLockoutMinutes(string username, HttpContext context, int lockoutMinutes = 15)
        {
            string timeKey = $"FailedLoginTime_{username}";

            if (context.Session[timeKey] == null)
                return 0;

            DateTime lockoutTime = (DateTime)context.Session[timeKey];
            TimeSpan elapsed = DateTime.Now - lockoutTime;
            int remaining = lockoutMinutes - (int)elapsed.TotalMinutes;

            return remaining > 0 ? remaining : 0;
        }

        /// <summary>
        /// Limpia intentos fallidos después de login exitoso
        /// </summary>
        public static void ClearFailedLoginAttempts(string username, HttpContext context)
        {
            context.Session[$"FailedLogin_{username}"] = null;
            context.Session[$"FailedLoginTime_{username}"] = null;
        }
    }
}



