using HotelListing3.API.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HotelListing3.API.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto);

        Task<bool> Login(LoginDto loginDto);
        //Task<string> CreateRefreshToken();
        //Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);
    }
}
