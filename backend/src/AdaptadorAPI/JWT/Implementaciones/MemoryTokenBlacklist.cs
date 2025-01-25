using AdaptadorAPI.Contratos;
using System.Collections.Concurrent;

namespace AdaptadorAPI.Implementaciones
{
    public class MemoryTokenBlacklist:ITokenBlacklist
    {
        private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

        public void BlacklistToken(string jti, DateTime expiry)
        {
            _blacklistedTokens.TryAdd(jti, expiry);
        }

        public bool IsTokenBlacklisted(string jti)
        {
            return _blacklistedTokens.TryGetValue(jti, out var expiry) && expiry > DateTime.UtcNow;
        }
    }
}
