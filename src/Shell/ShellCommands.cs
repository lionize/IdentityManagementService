using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TIKSN.Lionize.IdentityManagementService.Shell
{
    public class ShellCommands : IShellCommands
    {
        private readonly byte[] _buffer;
        private readonly ConfigurationDbContext _configurationDbContext;
        private readonly ILogger<ConfigurationDbContext> _logger;
        private readonly Random _random;

        public ShellCommands(
            Random random,
            ConfigurationDbContext configurationDbContext,
            ILogger<ConfigurationDbContext> logger)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _buffer = new byte[64];
            _configurationDbContext = configurationDbContext ?? throw new ArgumentNullException(nameof(configurationDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddApiSecretAsync(AddApiSecretOptions options)
        {
            var originalSecret = CreateOriginalSecret();

            try
            {
                var apiResource = await _configurationDbContext
                    .ApiResources
                    .Include(x => x.Secrets)
                    .Where(x => x.Id == options.Id)
                    .SingleAsync();

                apiResource.Secrets.Add(new ApiSecret { Value = originalSecret.Hashed, Type = IdentityServerConstants.SecretTypes.SharedSecret });

                await _configurationDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task AddClientSecret(AddClientSecretOptions options)
        {
            var originalSecret = CreateOriginalSecret();

            try
            {
                var client = await _configurationDbContext
                    .Clients
                    .Include(x => x.ClientSecrets)
                    .Where(x => x.Id == options.Id)
                    .SingleAsync();

                client.ClientSecrets.Add(new ClientSecret { Value = originalSecret.Hashed, Type = IdentityServerConstants.SecretTypes.SharedSecret });

                await _configurationDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private OriginalSecret CreateOriginalSecret()
        {
            _random.NextBytes(_buffer);

            var originalString = Convert.ToBase64String(_buffer);

            var originalStringSha256 = originalString.Sha256();

            _logger.LogInformation($"Secret: {originalString}");
            _logger.LogInformation($"Secret Hash: {originalStringSha256}");

            return new OriginalSecret(originalString, originalStringSha256);
        }
    }
}