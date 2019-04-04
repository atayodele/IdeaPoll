using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdeaSolution.Data.IGeneric.Auth;
using IdeaSolution.Data.Models;
using IdeaSolution.Services.Dto;
using IdeaSolution.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IdeaSolution.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(
            IAuthRepository repo,
            IConfiguration configuration,
            IMapper mapper,
            UserManager<AppUser> userManager)
        {
            _repo = repo;
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterForDto userRegisterDto)
        {             
            if (!string.IsNullOrEmpty(userRegisterDto.Email))
                userRegisterDto.Email = userRegisterDto.Email.ToLower();
            if (await _repo.UserExists(userRegisterDto.Email))
                ModelState.AddModelError("Email", "Email is already taken");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //var userToCreate = new AppUser().Assign(userRegisterDto);
            var userToCreate = _mapper.Map<AppUser>(userRegisterDto);
            var createUser = await _userManager.CreateAsync(userToCreate, userRegisterDto.Password);
            //var userToReturn = _mapper.Map<UserForDetailedDto>(createUser);
            if (createUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(userToCreate, "User");
                return Ok(new { email = userRegisterDto.Email, status = 1, message = "Registration Successful" });
            }
            return BadRequest();
        }
    }
}
