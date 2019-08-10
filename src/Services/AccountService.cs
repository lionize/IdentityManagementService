using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Lionize.IdentityManagementService.Models;

namespace TIKSN.Lionize.IdentityManagementService.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AccountService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public Task<IdentityResult> SignUpAsync(string username, string password, CancellationToken cancellationToken)
        {
            return userManager.CreateAsync(new ApplicationUser
            {
                UserName = username
            }, password);
        }
    }
}