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
    private ApiUser _user;


    private const string _loginProvider = "HotelListingApi";
    private const string _refreshToken = "RefreshToken";

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
        _user = await _userManager.FindByEmailAsync(loginDto.Email);
        bool isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);

        if (_user == null || isValidUser == false) return default;

        var token = await GenerateToken();

        // estoy mandando el id del user en UserId y en el token
        return new AuthResponseDto
        {
            Token = token,
            UserId = _user.Id,
            RefreshToken = await CreateRefreshToken()
        };

    }

    /// //////////////////////////////////////
    //////////////////////////////////////////////
    public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
    {
        _user = _mapper.Map<ApiUser>(userDto);
        _user.UserName = userDto.Email;

        var result = await _userManager.CreateAsync(_user, userDto.Password);

        if(result.Succeeded)
        {
            await _userManager.AddToRoleAsync(_user, "User");
        }

        // CREAR OTRO METODO DE REGISTRO Q CREE SOLO AMIN USERS  ( Y Q SOLO LOS ADMINS
        // PUEDAN ENTRAR EN ESE ENDPOINT )

        return result.Errors;
    }

    /// //////////////////////////////////////
    //////////////////////////////////////////////
    private async Task<string> GenerateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // los roles q el user tiene en la base de datos ( es una list de strings con los roles )
        var roles = await _userManager.GetRolesAsync(_user);
        // este basicamente me crea una lista de los roles del usuario sea 1 o +
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        // xsi cree claims manuales al registrar al user ( estarian guardados en la DB, xel identityUser )
        var userClaims = await _userManager.GetClaimsAsync(_user);

        // la lista de claims para el token
        var claims = new List<Claim>
            {
                // JwtRegisteredClaimNames.Sub es el subject a quien se le dio el token ( el usuario )
                // new Claim("uid", user.Id) --> por si quiero enviar el id de usuario en el token
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Id),
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

    /// //////////////////////////////////////
    //////////////////////////////////////////////
    public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        // AuthResponseDto ( request ) tiene UserId y Token-viejo
        var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
        var userName = tokenContent.Claims.ToList()
            .FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;

        _user = await _userManager.FindByNameAsync(userName);
        if (_user == null || _user.Id != request.UserId)
            return null;

        var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(
            _user, _loginProvider, _refreshToken, request.RefreshToken);

        if (isValidRefreshToken)
        {
            var token = await GenerateToken();
            return new AuthResponseDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };
        }

        // si no es valido el refreshT esta linea le genera una nueva security stamp y le hace logout
        await _userManager.UpdateSecurityStampAsync(_user);
        return null;
    }

    /// //////////////////////////////////////
    //////////////////////////////////////////////
    public async Task<string> CreateRefreshToken()
    {
        await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);

        // genero el token
        var newRefreshToken = await _userManager.GenerateUserTokenAsync(
            _user, _loginProvider, _refreshToken);

        // guardo el token en DB
        var result = await _userManager.SetAuthenticationTokenAsync(
            _user, _loginProvider, _refreshToken, newRefreshToken);

        return newRefreshToken;
    }
}
