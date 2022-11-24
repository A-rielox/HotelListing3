﻿using HotelListing3.API.Contracts;
using HotelListing3.API.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing3.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAuthManager _authManager;

    public AccountController(IAuthManager authManager)
    {
        _authManager = authManager;
    }

    /// //////////////////////////////////////
    //////////////////////////////////////////////
    /// POST: api/Account/register
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Register([FromBody] ApiUserDto apiUserDto)
    {
        var errors = await _authManager.Register(apiUserDto);

        // el ModelState es donde estan guardados esos errores q se manejan xdefault
        // como cundo tengo nombre [Required] y no lo pongo y me manda el error de 
        // q tengo q ponerlo
        if (errors.Any())
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            // mando el bad request con los errores que agregue, q van a venir del 
            // AuthManager.cs q es el que tiene la logica del business
            return BadRequest(ModelState);
        }

        return Ok();
    }

    /// //////////////////////////////////////
    //////////////////////////////////////////////
    /// POST: api/Account/login
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
    {
        var authResponse = await _authManager.Login(loginDto);
        if (authResponse == null)
        {
            return Unauthorized();
        }

        // en authResponse esta el token y userId
        return Ok(authResponse);
    }
}
