using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using IdeaSolution.Data;
using IdeaSolution.Data.IGeneric;
using IdeaSolution.Data.IGeneric.IdeaRepo;
using IdeaSolution.Data.IGeneric.UserRepo;
using IdeaSolution.Data.Models;
using IdeaSolution.Services.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdeaSolution.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/users/{userId}/idea")]
    public class IdeaController : ControllerBase
    {
        private readonly string[] ACCEPTED_FILE_TYPES = new[] { ".pdf", ".docx", ".doc" };
        private readonly IIdeaRepository _ideaRepo;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;
        private readonly IHostingEnvironment _host;
        private readonly IRepo _repo;

        public IdeaController(
                    IIdeaRepository ideaRepo, 
                    IMapper mapper,
                    IUserRepository userRepo,
                    IHostingEnvironment host,
                    IRepo repo)
        {
            _ideaRepo = ideaRepo;
            _mapper = mapper;
            _userRepo = userRepo;
            _host = host;
            _repo = repo;
        }
        [HttpGet("userIdea")]
        public async Task<IActionResult> GetUserIdeas(string userId)
        {
            var user = await _userRepo.GetUser(userId);
            if (user == null)
                return BadRequest("Could not find user");
            var currerntUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (currerntUserId != user.Email)
                return Unauthorized();
            var ideas = await _ideaRepo.GetUserIdeas(userId);
            var ideaToReturn = _mapper.Map<IEnumerable<IdeaForListDto>>(ideas);
            return Ok(ideaToReturn);
        }
        [HttpGet]
        public async Task<IActionResult> GetIdeas()
        {
            var ideas = await _ideaRepo.GetAll();
            var ideaToReturn = _mapper.Map<IEnumerable<IdeaForListDto>>(ideas);
            return Ok(ideaToReturn);
        }
        [HttpPost]
        public async Task<IActionResult> AddIdea(string userId, [FromForm]IdeaCreationDto ideaCreation)
        {
            var user = await _userRepo.GetUser(userId);
            if (user == null)
                return BadRequest("Could not find user");
            var currerntUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (currerntUserId != user.Email)
                return Unauthorized();
            var filesData = ideaCreation.File;
            if (filesData == null) return BadRequest("Null File");
            if (filesData.Length == 0)
            {
                return BadRequest("Empty File");
            }
            if (filesData.Length > 10 * 1024 * 1024) return BadRequest("Max file size exceeded.");
            if (!ACCEPTED_FILE_TYPES.Any(s => s == Path.GetExtension(filesData.FileName).ToLower())) return BadRequest("Invalid file type.");
            var uploadFilesPath = Path.Combine(_host.WebRootPath, "ideaPollFolder");
            if (!Directory.Exists(uploadFilesPath))
                Directory.CreateDirectory(uploadFilesPath);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(filesData.FileName);
            var filePath = Path.Combine(uploadFilesPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await filesData.CopyToAsync(stream);
            }

            ideaCreation.FilePath= "~/ideaPollFolder/" + fileName.ToString();

            var idea = _mapper.Map<Idea>(ideaCreation); 
            idea.User = user;
            idea.IsRead = false;
            // add photo to db
            user.Ideas.Add(idea);
            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<IdeaForReturnDto>(idea);
                return CreatedAtRoute("GetPhoto", new { id = idea.Id }, photoToReturn);
            }
            return BadRequest("Could not add the photo to db");
        }
    }
}