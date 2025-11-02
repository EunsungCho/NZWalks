using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public interface IWalkRepository
    {
        Task<Walk?> CreateAsync(Walk walk);
        Task<List<Walk>?> GetAllWalksAsync();
        Task<Walk?> GetWalkByIdAsync(Guid id);
        Task<Walk?> UpdateWalkAsync(Walk walk);
        Task<Walk?> DeleteWalkAsync(Guid id);
    }
}
