using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalkDbContext dbContext;

        public SQLWalkRepository(NZWalkDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Walk?> CreateAsync(Walk walk)
        {
            var returnedWalk = await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();
            var id = returnedWalk.Entity.Id;
            var savedWalk = await dbContext.Walks.FirstOrDefaultAsync(w => w.Id == id);
            
            return savedWalk;
        }

        public async Task<Walk?> DeleteWalkAsync(Guid id)
        {
            var existingWalk = await dbContext.Walks.FirstOrDefaultAsync(w => w.Id == id);
            if (existingWalk == null)
                return null;
            dbContext.Walks.Remove(existingWalk);
            await dbContext.SaveChangesAsync();
            return existingWalk;
        }

        public async Task<List<Walk>?> GetAllWalksAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 1000)
        {
            //var walks = await dbContext.Walks                
            //    .Include(nameof(Difficulty))
            //    .Include(nameof(Region))
            //    .ToListAsync();
            //return walks;

            var walks = dbContext.Walks.Include(nameof(Difficulty)).Include(nameof(Region)).AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.ToUpper().Equals("NAME"))
                    walks = walks.Where(w => w.Name.Contains(filterQuery));
                else if (filterOn.ToUpper().Equals("DESCRIPTION"))
                    walks = walks.Where(w => w.Description.Contains(filterQuery));
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(w => w.Name) : walks.OrderByDescending(w => w.Name);
                }
                else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(w => w.LengthInKm) : walks.OrderByDescending(w => w.LengthInKm);
                }
            }

            // Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public async Task<Walk?> GetWalkByIdAsync(Guid id)
        {
            var walk = await dbContext.Walks
                .Include(nameof(Difficulty))
                .Include(nameof(Region))
                .FirstOrDefaultAsync(w => w.Id == id);
            return walk;
        }

        public async Task<Walk?> UpdateWalkAsync(Walk walk)
        {
            var existingWalk = await dbContext.Walks.FirstOrDefaultAsync(w => w.Id == walk.Id);
            if (existingWalk == null)
                return null;

            existingWalk.Name = walk.Name;
            existingWalk.LengthInKm = walk.LengthInKm;
            existingWalk.Description = walk.Description;
            existingWalk.DifficultyId = walk.DifficultyId;
            existingWalk.RegionId = walk.RegionId;
            existingWalk.WalkImageUrl = walk.WalkImageUrl;

            await dbContext.SaveChangesAsync();
            return existingWalk;
        }
    }
}
