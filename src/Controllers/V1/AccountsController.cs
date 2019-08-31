using Lionize.IdentityManagementService.ApiModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Lionize.IdentityManagementService.Services;
using TIKSN.Lionize.IdentityManagementService.Validators;

namespace TIKSN.Lionize.IdentityManagementService.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountsController
    {
        private readonly IAccountService accountService;

        public AccountsController(IAccountService accountService)
        {
            this.accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [HttpPost("SignUp")]
        public async Task<SignUpResponse> SignUp([FromBody]SignUpRequest request, CancellationToken cancellationToken)
        {
            var validator = new SignUpRequestValidator();

            var validation = validator.Validate(request);

            if (validation.IsValid)
            {
                var result = await accountService.SignUpAsync(request.Username, request.Password, cancellationToken);

                if (result.Succeeded)
                    return new SignUpResponse { IsError = false, ErrorMessage = string.Empty };
                else
                    return new SignUpResponse { IsError = true, ErrorMessage = result.Errors.First().Description };
            }
            else
            {
                return new SignUpResponse { IsError = true, ErrorMessage = validation.Errors.First().ErrorMessage };
            }
        }
    }
}