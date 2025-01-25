namespace AdaptadorAPI.Contratos
{
    public interface ITokenBlacklist
    {
        void BlacklistToken(string jti, DateTime expiry);
        bool IsTokenBlacklisted(string jti);
    }
}
