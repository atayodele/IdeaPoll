using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdeaSolution.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/users/{userId}/photos")]
    public class PhotosController : ControllerBase
    {
        private readonly string[] ACCEPTED_FILE_TYPES = new[] { ".jpg", ".jpeg", ".png" };
        private readonly IPhotoRepository _photoRepo;
        private readonly IMapper _mapper;
        private readonly IRepo _repo;
        private readonly IUserRepository _userRepo;
        private readonly IHostingEnvironment _host;

        public PhotosController(IPhotoRepository photoRepo,
                                IMapper mapper,
                                IRepo repo,
                                IUserRepository userRepo,
                                IHostingEnvironment host)
        {
            _photoRepo = photoRepo;
            _mapper = mapper;
            _repo = repo;
            _userRepo = userRepo;
            _host = host;
        }
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _photoRepo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(string userId, PhotoForCreationDto photoDto)
        {
            var user = await _userRepo.GetUser(userId);
            if (user == null)
                return BadRequest("Could not find user");
            var currerntUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (currerntUserId != user.Email)
                return Unauthorized();
            var filesData = photoDto.File;
            if (filesData == null) return BadRequest("Null File");
            if (filesData.Length == 0)
            {
                return BadRequest("Empty File");
            }
            if (filesData.Length > 10 * 1024 * 1024) return BadRequest("Max file size exceeded.");
            if (!ACCEPTED_FILE_TYPES.Any(s => s == Path.GetExtension(filesData.FileName).ToLower())) return BadRequest("Invalid file type.");
            var uploadFilesPath = Path.Combine(_host.WebRootPath, "uploads");
            if (!Directory.Exists(uploadFilesPath))
                Directory.CreateDirectory(uploadFilesPath);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(filesData.FileName);
            var filePath = Path.Combine(uploadFilesPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await filesData.CopyToAsync(stream);
            }
            
            photoDto.Url = "~/uploads/" + fileName.ToString();
            var photo = _mapper.Map<Photo>(photoDto);
            photo.User = user;
            //check if user has main photo
            if (!user.Photos.Any(m => m.IsMain))
                photo.IsMain = true; //set photo to main photo if there is no main photo
            // add photo to db
            user.Photos.Add(photo);
            if(await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }
            return BadRequest("Could not add the photo to db");
        }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(string userId, int id)
        {
            var user = await _userRepo.GetUser(userId);
            if (user == null)
                return BadRequest("Could not find user");
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (currentUserId != user.Email)
                return Unauthorized();
            //get photo by id
            var photoFromRepo = await _photoRepo.GetPhoto(id);
            //check if pix is in db
            if (photoFromRepo == null)
                return NotFound();
            //check if is the main photo
            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");
            //if not, get the current main photo
            var currentMainPhoto = await _photoRepo.GetMainPhotoForUser(userId);
            //set the current photo to false
            if (currentMainPhoto != null)
                currentMainPhoto.IsMain = false;
            // change the new photo to true
            photoFromRepo.IsMain = true;
            if (await _repo.SaveAll())
                return NoContent();
            return BadRequest("Could not set photo to Main");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(string userId, int id)
        {
            var user = await _userRepo.GetUser(userId);
            if (user == null)
                return BadRequest("Could not find user");
            if (user.Email != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();
            // get and check image by id
            var photoFromRepo = await _photoRepo.GetPhoto(id);
            if (photoFromRepo == null)
                return NotFound();
            //cannot delete main photo
            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");
            _repo.Delete(photoFromRepo);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest("Failed to delete the photo");
        }
    }
}