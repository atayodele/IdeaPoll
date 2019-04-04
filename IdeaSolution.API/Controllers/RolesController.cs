using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdeaSolution.Data.IGeneric.Auth;
using IdeaSolution.Services.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdeaSolution.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IAuthRepository _repo;

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IAuthRepository repo)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _repo = repo;
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
            var userToReturn = _mapper.Map<RoleForListDto>(createRole);
            if (createRole.Succeeded)
            {
                //return Ok();
                return CreatedAtRoute("GetRole", new { id = create.Id }, userToReturn);
            }
            return BadRequest("Failed to create");
        }
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _repo.GetRoles();
            var rolesToReturn = _mapper.Map<IEnumerable<RoleForListDto>>(roles);
            return Ok(rolesToReturn);
        }
        [HttpGet("{id}", Name = "GetRole")]
        public async Task<IActionResult> GetRole(string id)
        {
            var role = await _repo.GetRole(id);
            var roleToReturn = _mapper.Map<RoleForListDto>(role);
            return Ok(roleToReturn);
        }
    }
}