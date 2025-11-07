using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        [HttpPost]
        [Route("Upload")]
        // Post: /api/images/Upload
        public async Task<IActionResult> Upload([FromForm]ImageUploadRequestDto request)
        {
            ValidateFileUpload(request);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);                
            }

            // Convert DTO to Domain model
            var imageDomainModel = new Image
            {
                File = request.File,
                FileName = request.FileName,
                FileExtension = Path.GetExtension(request.File.FileName),
                FileSizeInBytes = request.File.Length,
                FileDescription = request.FileDescription
            };

            // User repository to upload image;
            //var image = await imageRepository.Upload(imageDomainModel);
            await imageRepository.Upload(imageDomainModel);
            return Ok(imageDomainModel);
        }

        private void ValidateFileUpload(ImageUploadRequestDto requestDto)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            if (!allowedExtensions.Contains(Path.GetExtension(requestDto.File.FileName))) {
                ModelState.AddModelError("file", "Unsupported file extension.");
            };

            if (requestDto.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size more than 10MB, please upload a smaller size file.");
            }
        }
    }
}
