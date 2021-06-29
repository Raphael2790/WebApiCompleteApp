using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RSS.Business.Utils;
using RSS.WebApi.DTOs;
using System.Threading.Tasks;

namespace RSS.WebApi.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<ServiceResult<LoginResponseDTO>> CreateIdentityUser(RegisterUserDTO user);
        Task<ServiceResult<LoginResponseDTO>> LogInIdentityUser(LoginUserDTO loginUser);
    }
}
