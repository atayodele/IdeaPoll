using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using IdeaSolution.Data.IGeneric;
using IdeaSolution.Data.IGeneric.Auth;
using IdeaSolution.Data.IGeneric.UserRepo;
using IdeaSolution.Data.Models;
using IdeaSolution.Services.Dto;
using IdeaSolution.Services.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdeaSolution.API.Controllers.Administrator
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IAuthRepository _auth;
        private readonly IRepo _repo;
        private readonly UserManager<AppUser> _userManager;

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IAuthRepository auth,
            IRepo repo,
            UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _auth = auth;
            _repo = repo;
            _userManager = userManager;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddRole([FromBody] RoleForDto roleForDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var role = await _roleManager.FindByNameAsync(roleForDto.Name);
            if(role != null)
            {
                return BadRequest($"{roleForDto.Name} role already exist!");
            }
            //role = new IdentityRole(roleForDto.Name);
            var create = _mapper.Map<IdentityRole>(roleForDto);
            var createRole = await _roleManager.CreateAsync(create);
            if (createRole.Succeeded)
            {
                return Ok(new { roleName = roleForDto.Name, id = create.Id, message = "Role Created Successfully" });
            }
            return BadRequest("Failed to create");
        }
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _auth.GetRoles();
            var rolesToReturn = _mapper.Map<IEnumerable<RoleForListDto>>(roles);
            return Ok(rolesToReturn);
        }
        [HttpGet("{id}", Name = "GetRole")]
        public async Task<IActionResult> GetRole(string id)
        {
            var role = await _auth.GetRole(id);
            if (role == null)
                return BadRequest("Role is not found");
            var roleToReturn = _mapper.Map<RoleForListDto>(role);
            return Ok(roleToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleForUpdateDto roleForUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var role = await _auth.GetRole(id);
            if (role == null)
                return NotFound($"Could not found role with an ID of {id}");
            role.Name = roleForUpdateDto.Name;
            role.NormalizedName = roleForUpdateDto.Name;
            var res = await _roleManager.UpdateAsync(role);
            if(res.Succeeded)
                return NoContent();
            throw new Exception($"Updating role {id} failed on save");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var role = await _auth.GetRole(id);
            if (role == null)
                return NotFound($"Could not found role with an ID of {id}");
            var delete = await _roleManager.DeleteAsync(role);
            if (delete.Succeeded)
                return Ok("Role Deleted Successfully");
            return BadRequest($"Failed to delete role with Id {id}");
        }
        [HttpGet("GetUserByRole/{id}")]
        public async Task<IActionResult> GetUserByRole(string id)
        {
            var role = await _auth.GetRole(id);
            if (role == null)
                return NotFound($"Could not found role with an ID of {id}");
            var findUser = await _userManager.GetUsersInRoleAsync(role.Name);
            var roleToReturn = _mapper.Map<IEnumerable<UserRoleListDto>>(findUser);
            return Ok(roleToReturn);
        }
    }
}