using IdentityServer4.Models;
using System;
using System.Threading.Tasks;

namespace TIKSN.Lionize.IdentityManagementService.Shell
{
    public class ShellCommands : IShellCommands
    {
        private readonly byte[] _buffer;
        private readonly Random _random;

        public ShellCommands(Random random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _buffer = new byte[64];
        }

        public Task AddApiSecretAsync(AddApiSecretOptions options)
        {
            var originalSecret = CreateOriginalSecret();
            throw new NotImplementedException();
        }

        public Task AddClientSecret(AddClientSecretOptions options)
        {
            var originalSecret = CreateOriginalSecret();
            throw new NotImplementedException();
        }

        private OriginalSecret CreateOriginalSecret()
        {
            _random.NextBytes(_buffer);

            var originalString = Convert.ToBase64String(_buffer);

            var originalStringSha256 = originalString.Sha256();

            return new OriginalSecret(originalString, originalStringSha256);
        }
    }
}