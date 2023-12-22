namespace WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.Secure
{
    public interface ICipherService
    {
        string Encrypt(string input);
        string Decrypt(string cipherText);
    }
}
