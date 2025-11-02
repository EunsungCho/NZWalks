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

        public async Task<List<Walk>?> GetAllWalksAsync()
        {
            var walks = await dbContext.Walks
                .Include(nameof(Difficulty))
                .Include(nameof(Region))
                .ToListAsync();
            return walks;
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
