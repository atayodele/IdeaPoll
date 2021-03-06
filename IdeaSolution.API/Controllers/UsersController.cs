﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using IdeaSolution.Data.IGeneric;
using IdeaSolution.Data.IGeneric.UserRepo;
using IdeaSolution.Data.Models;
using IdeaSolution.Services.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdeaSolution.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;
        private readonly IRepo _repo;
        private readonly UserManager<AppUser> _userManager;

        public UsersController(
            IMapper mapper,
            IUserRepository userRepo,
            IRepo repo,
            UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _userRepo = userRepo;
            _repo = repo;
            _userManager = userManager;
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userRepo.GetUser(id);
            if (user == null)
                return BadRequest($"User with {id} is not found");
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserForUpdateDto userForUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = await _userRepo.GetUser(id);
            if (userFromRepo == null)
                return NotFound($"Could not find user with an ID of {id}");
            if (currentUser != userFromRepo.Email)
                return Unauthorized();
            _mapper.Map(userForUpdateDto, userFromRepo);
            if(await _repo.SaveAll())
            {
                return Ok("User Profile updated successfully");
            }
            return BadRequest($"Updating user {userForUpdateDto.Firstname} failed oon save");
        }
    }
}