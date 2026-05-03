namespace GamestoreApi.Service
{
    public interface ITokenGenerator
    {
        string GenerateToken(string subject, int expiresInSeconds = 3600);
    }
}
