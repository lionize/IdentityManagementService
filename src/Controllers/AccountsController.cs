using Lionize.IdentityManagementService.ApiModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Lionize.IdentityManagementService.Validators;

namespace TIKSN.Lionize.IdentityManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController
    {
        [HttpPost("SignUp")]
        public async Task<SignUpResponse> SignUp([FromBody]SignUpRequest request)
        {
            var validator = new SignUpRequestValidator();

            var validation = validator.Validate(request);

            if (validation.IsValid)
            {
                throw new NotImplementedException();
            }
            else
            {
                return new SignUpResponse { IsError = true, ErrorMessage = validation.Errors.First().ErrorMessage };
            }
        }
    }
}