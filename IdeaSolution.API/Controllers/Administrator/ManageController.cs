﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using IdeaSolution.Data.IGeneric;
using IdeaSolution.Data.IGeneric.IdeaRepo;
using IdeaSolution.Data.IGeneric.UserRepo;
using IdeaSolution.Data.Models;
using IdeaSolution.Services.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdeaSolution.API.Controllers.Administrator
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ManageController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;
        private readonly IRepo _repo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IIdeaRepository _ideaRepo;

        public ManageController(
            IMapper mapper,
            IUserRepository userRepo,
            IRepo repo,
            UserManager<AppUser> userManager,
            IIdeaRepository ideaRepo)
        {
            _mapper = mapper;
            _userRepo = userRepo;
            _repo = repo;
            _userManager = userManager;
            _ideaRepo = ideaRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepo.GetUsers();
            var userToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(userToReturn);
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
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] UserForUpdateDto userForUpdateDto)
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
            if (await _repo.SaveAll())
            {
                return Ok("User Profile updated successfully");
            }
            return BadRequest($"Updating user {userForUpdateDto.Firstname} failed oon save");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userRepo.GetUser(id);
            if (user == null)
                return NotFound($"Could not found role with an ID of {id}");
            var delete = await _userManager.DeleteAsync(user);
            if (delete.Succeeded)
                return Ok("User Deleted Successfully");
            return BadRequest($"Failed to delete user with Id {id}");
        }
        [HttpGet("getIdeas")]
        public async Task<IActionResult> GetIdeas()
        {
            var ideas = await _ideaRepo.GetAll();
            var ideaToReturn = _mapper.Map<IEnumerable<IdeaForListDto>>(ideas);
            return Ok(ideaToReturn);
        }
        [HttpGet("getIdeaById/{id}")]
        public async Task<IActionResult> GetIdeaById(long id)
        {
            var idea = await _ideaRepo.GetIdea(id);
            var ideaToReturn = _mapper.Map<IdeaForListDto>(idea);
            return Ok(ideaToReturn);
        }
        [HttpDelete("IdeaDelete/{id}")]
        public async Task<IActionResult> IdeaDelete(long id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var idea = await _ideaRepo.GetIdea(id);
            if (idea == null)
                return NotFound($"Could not found idea with an ID of {id}");
            _repo.Delete(idea);
            if (await _repo.SaveAll())
                return Ok("Idea Deleted Successfully");
            return BadRequest($"Failed to delete idea with Id {id}");
        }
    }
}