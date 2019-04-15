using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
using Microsoft.IdentityModel.Tokens;

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
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            IAuthRepository repo,
            IConfiguration configuration,
            IMapper mapper,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _repo = repo;
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
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
            var userToCreate = _mapper.Map<AppUser>(userRegisterDto);
            var createUser = await _userManager.CreateAsync(userToCreate, userRegisterDto.Password);
            //var userToReturn = _mapper.Map<UserForDetailedDto>(createUser);
            if (createUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(userToCreate, "User");
                //await _userManager.AddToRolesAsync(userToCreate, userRegisterDto.Roles);
                return Ok(new { email = userRegisterDto.Email, status = 1, message = "Registration Successful" });
            }
            return BadRequest();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Email, userForLoginDto.Password);
            if (userFromRepo == null)
                return Unauthorized();
            var userRoles = await _userManager.GetRolesAsync(userFromRepo);
            //generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var fullname = userFromRepo.Firstname + " " + userFromRepo.Othername;
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);
            var claims = new List<Claim>
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, userFromRepo.Email),
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, fullname)
                    };
            foreach (var roleName in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }
            var tokenDescriptor = new JwtSecurityToken(
                    issuer: "http://gosmarticle.com",
                    audience: "http://api.gosmarticle.com",
                    expires: DateTime.Now.AddDays(1),
                    claims: claims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                    );
            
            var token = tokenHandler.WriteToken(tokenDescriptor);

            var Expires = tokenDescriptor.ValidTo;

            var user = _mapper.Map<UserForListDto>(userFromRepo);

            return Ok(new { token, user, roles = userRoles});
        }
    }
}
