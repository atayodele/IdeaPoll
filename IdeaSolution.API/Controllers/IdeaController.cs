using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdeaSolution.Data.IGeneric.IdeaRepo;
using IdeaSolution.Services.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdeaSolution.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdeaController : ControllerBase
    {
        private readonly IIdeaRepository _ideaRepo;
        private readonly IMapper _mapper;

        public IdeaController(IIdeaRepository ideaRepo, IMapper mapper)
        {
            _ideaRepo = ideaRepo;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetIdeas()
        {
            var ideas = await _ideaRepo.GetAll();
            var ideaToReturn = _mapper.Map<IEnumerable<IdeaForListDto>>(ideas);
            return Ok(ideaToReturn);
        }
        [HttpGet("{id}", Name = "GetIdea")]
        public async Task<IActionResult> GetIdea(long id)
        {
            var idea = await _ideaRepo.GetIdea(id);
            var ideaToReturn = _mapper.Map<IdeaForListDto>(idea);
            return Ok(ideaToReturn);
        }
        //[HttpPost("addIdea")]
        //public async Task<IActionResult> AddIdea(IdeaCreationDto ideaCreation)
        //{

        //}
    }
}