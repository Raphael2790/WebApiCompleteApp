using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSS.Business.Interfaces;
using RSS.WebApi.DTOs;
using RSS.WebApi.Services.Interfaces;
using System.Threading.Tasks;

namespace RSS.WebApi.Controllers
{
    [Route("api")]
    public class AuthController : MainController
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService,
                              INotifiable notifiable) : base(notifiable)
        {
            _identityService = identityService;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RegisterUser(RegisterUserDTO registerUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _identityService.CreateIdentityUser(registerUser);

            if(!result.Success)
            return CustomResponse(registerUser);


            return CustomResponse(result.Result);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> LoginUser(LoginUserDTO loginUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _identityService.LogInIdentityUser(loginUser);

            if (!result.Success)
            return CustomResponse(loginUser);

            return CustomResponse(result.Result);
        }
        
    }
}
