using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        // Create walk
        // Post: /api/walks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addwalkRequestDto)
        {
            // Map DTO to Domain model
            var walkDomainModel = mapper.Map<Walk>(addwalkRequestDto);
            var walk = await walkRepository.CreateAsync(walkDomainModel);

            return Ok(mapper.Map<WalkDto>(walk));
        }

        // Get walks
        [HttpGet]
        public async Task<IActionResult> GetWalks()
        {
            var walksDomain = await walkRepository.GetAllWalksAsync();
            var walksDto = mapper.Map<List<WalkDto>>(walksDomain);
            if (walksDto == null)
                return NotFound();

            return Ok(walksDto);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetWalkById([FromRoute]Guid id)
        {
            var walk = await walkRepository.GetWalkByIdAsync(id);
            if (walk == null)
                return NotFound();

            var walkDto = mapper.Map<WalkDto>(walk);

            return Ok(walkDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalk([FromRoute]Guid id, [FromBody]UpdateWalkDto updateWalkDto)
        {
            var walk = mapper.Map<Walk>(updateWalkDto);
            walk.Id = id;
            var updatedWalk = await walkRepository.UpdateWalkAsync(walk);
            if (updatedWalk == null)
                return NotFound();
            var walkDto = mapper.Map<WalkDto>(updatedWalk);
            return Ok(walkDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalk([FromRoute]Guid id)
        {
            var deletedWalk = await walkRepository.DeleteWalkAsync(id);
            if (deletedWalk == null)
                return NotFound();
            var walkDto = mapper.Map<WalkDto>(deletedWalk);
            return Ok(walkDto);
        }
    }
}
