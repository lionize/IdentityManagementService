using System.Threading.Tasks;

namespace TIKSN.Lionize.IdentityManagementService.Shell
{
    public interface IShellCommands
    {
        Task AddApiSecretAsync(AddApiSecretOptions options);

        Task AddClientSecretAsync(AddClientSecretOptions options);

        Task Sha256Async(Sha256Options options);
    }
}