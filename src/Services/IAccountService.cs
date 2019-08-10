using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TIKSN.Lionize.IdentityManagementService.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> SignUpAsync(string username, string password, CancellationToken cancellationToken);
    }
}