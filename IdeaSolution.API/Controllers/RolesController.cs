using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
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
            role = new IdentityRole(roleForDto.Name);
            var createRole = await _roleManager.CreateAsync(role);
            if (createRole.Succeeded)
            {
                return Ok();
            }
            return BadRequest("Failed to create");
        }
    }
}