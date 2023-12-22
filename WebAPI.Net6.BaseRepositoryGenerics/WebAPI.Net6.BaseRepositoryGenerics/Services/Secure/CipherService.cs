using Microsoft.AspNetCore.DataProtection;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.Secure;

namespace WebAPI.Net6.BaseRepositoryGenerics.Services.Secure
{
    public class CipherService : ICipherService
    {
        private readonly ILogger _logger;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string purpose = "My.SystemSettings";

        public CipherService(IDataProtectionProvider dataProtectionProvider, ILogger<CipherService> logger)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _logger = logger;
        }

        public string Encrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(purpose);
            var encryptedString = protector.Protect(input);
            _logger.LogInformation("The encrypted string is:" + encryptedString);
            return encryptedString;
        }

        public string Decrypt(string cipherText)
        {
            var protector = _dataProtectionProvider.CreateProtector(purpose);
            return protector.Unprotect(cipherText);
        }
    }
}
