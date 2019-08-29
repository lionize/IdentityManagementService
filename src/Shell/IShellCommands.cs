using System.Threading.Tasks;

namespace TIKSN.Lionize.IdentityManagementService.Shell
{
    public interface IShellCommands
    {
        Task AddApiSecretAsync(AddApiSecretOptions options);

        Task AddClientSecret(AddClientSecretOptions options);
    }
}