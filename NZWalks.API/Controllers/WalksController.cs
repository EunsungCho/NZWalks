using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
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
        [ValidateModelAttribute]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addwalkRequestDto)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            // Map DTO to Domain model
            var walkDomainModel = mapper.Map<Walk>(addwalkRequestDto);
            var walk = await walkRepository.CreateAsync(walkDomainModel);

            return Ok(mapper.Map<WalkDto>(walk));
        }

        // Get walks
        // GET: /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]string? filterOn, [FromQuery]string? filterQuery,
            [FromQuery]string? sortBy, [FromQuery]bool? isAscending,
            [FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 1000)
        {
            var walksDomain = await walkRepository.GetAllWalksAsync(filterOn, filterQuery, sortBy, isAscending ?? true,
                pageNumber, pageSize);
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
        [ValidateModelAttribute]
        public async Task<IActionResult> UpdateWalk([FromRoute]Guid id, [FromBody]UpdateWalkDto updateWalkDto)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

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
