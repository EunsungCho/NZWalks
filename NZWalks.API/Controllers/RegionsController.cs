using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalkDbContext dbContext;

        public RegionsController(NZWalkDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var regions = dbContext.Regions.ToList();

            return Ok(regions);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById([FromRoute]Guid id)
        {
            var region = dbContext.Regions.FirstOrDefault(r => r.Id == id);
            //var region = dbContext.Regions.Find(id);
            if (region == null)
            {
                return NotFound();
            }
            return Ok(region);
        }
    }
}
