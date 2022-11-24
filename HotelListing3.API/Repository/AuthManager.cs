using AutoMapper;
using HotelListing3.API.Contracts;
using HotelListing3.API.Data;
using HotelListing3.API.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// "UserManager" default library que ayuda con el registro ( parece q actua
// como el context )

namespace HotelListing3.API.Repository;

public class AuthManager : IAuthManager
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthManager(IMapper mapper, 
                       UserManager<ApiUser> userManager,
                       IConfiguration configuration) // la config que tiene la secret-key en Program.cs
    {
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
    }

    /// //////////////////////////////////////
    //////////////////////////////////////////////
    public async Task<AuthResponseDto> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        bool isValidUser = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (user == null || isValidUser == false) return default;

        var token = await GenerateToken(user);

        // estoy mandando el id del user en UserId y en el token
        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id
        };

    }

    /// //////////////////////////////////////
    //////////////////////////////////////////////
    public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
    {
        var user = _mapper.Map<ApiUser>(userDto);
        user.UserName = userDto.Email;

        var result = await _userManager.CreateAsync(user, userDto.Password);

        if(result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        return result.Errors;
    }

    /// //////////////////////////////////////
    //////////////////////////////////////////////
    private async Task<string> GenerateToken(ApiUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // los roles q el user tiene en la base de datos ( es una list de strings con los roles )
        var roles = await _userManager.GetRolesAsync(user);
        // este basicamente me crea una lista de los roles del usuario sea 1 o +
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        // xsi cree claims manuales al registrar al user ( estarian guardados en la DB, xel identityUser )
        var userClaims = await _userManager.GetClaimsAsync(user);

        // la lista de claims para el token
        var claims = new List<Claim>
            {
                // JwtRegisteredClaimNames.Sub es el subject a quien se le dio el token ( el usuario )
                // new Claim("uid", user.Id) --> por si quiero enviar el id de usuario en el token
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
            }.Union(userClaims).Union(roleClaims);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}