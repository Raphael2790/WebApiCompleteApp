using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RSS.Business.Interfaces;
using RSS.Business.Services;
using RSS.Business.Utils;
using RSS.WebApi.DTOs;
using RSS.WebApi.Extensions;
using RSS.WebApi.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RSS.WebApi.Services
{
    public class IdentityService : BaseService, IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppSettings _appSettings;

        public IdentityService(UserManager<IdentityUser> userManager, 
                                SignInManager<IdentityUser> signInManager,
                                INotifiable notifiable,
                                IOptions<AppSettings> appSettings) : base (notifiable)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
        }

        private ClaimsIdentity GetUserIdentityClaims(IdentityUserDTO identityUser)
        {
            var identityClaims = new ClaimsIdentity();

            identityUser.Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, identityUser.User.Id));
            identityUser.Claims.Add(new Claim(JwtRegisteredClaimNames.Email, identityUser.User.Email));
            identityUser.Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            identityUser.Claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            identityUser.Claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var role in identityUser.Roles)
            {
                identityUser.Claims.Add(new Claim("role", role));
            }

            identityClaims.AddClaims(identityUser.Claims);

            return identityClaims;
        }

        private string GetUserToken(IdentityUserDTO identityUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.EmittedBy,
                Audience = _appSettings.ValidOn,
                Subject = GetUserIdentityClaims(identityUser),
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.ExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);

            return encodedToken;
        }

        private async Task<LoginResponseDTO> GetLoginUserResponse(string email)
        {
            var identityUser = await GetRegisterIdentityUser(email);

            var response = new LoginResponseDTO
            {
                AccessToken = GetUserToken(identityUser),
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpirationMinutes).TotalSeconds,
                UserToken = new UserTokenDTO
                {
                    Id = identityUser.User.Id,
                    Email = identityUser.User.Email,
                    Claims = identityUser.Claims.Select(c => new ClaimDTO { Type = c.Type, Value = c.Value })
                }
            };

            return response;
        }

        private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);


        public async Task<ServiceResult<LoginResponseDTO>> CreateIdentityUser(RegisterUserDTO registerUser)
        {
            ServiceResult<LoginResponseDTO> serviceResult = new ServiceResult<LoginResponseDTO>();

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                serviceResult.SetSuccess(await GetLoginUserResponse(registerUser.Email));
            }

            if (result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    Notify(error.Description);
                }
                serviceResult.SetError(result.Errors.First().Description);
            }

            return serviceResult;
        }

        public async Task<ServiceResult<LoginResponseDTO>> LogInIdentityUser(LoginUserDTO loginUser)
        {
            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            ServiceResult<LoginResponseDTO> serviceResult = new ServiceResult<LoginResponseDTO>();

            if (result.Succeeded)
            {
                return serviceResult.SetSuccess(await GetLoginUserResponse(loginUser.Email));
            }
            if (result.IsLockedOut)
            {
                Notify("Usuário bloquado temporariamente");
                return serviceResult.SetError("Usuário bloquado temporariamente");
            }

            Notify("Usuário ou senha inválidos");
            return serviceResult.SetError("Usuário ou senha inválidos");
        }

        private async Task<IdentityUserDTO> GetRegisterIdentityUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            return new IdentityUserDTO
            {
                User = user,
                Claims = claims,
                Roles  = userRoles
            };
        }
    }

}
