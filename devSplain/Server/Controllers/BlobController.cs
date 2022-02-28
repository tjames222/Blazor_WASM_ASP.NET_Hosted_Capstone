using devSplain.Server.Services;
using devSplain.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;

namespace devSplain.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BlobController : Controller
    {
        private readonly IBlobService _blobService;
        private readonly IWebHostEnvironment _env;

        public BlobController(IBlobService blobService, IWebHostEnvironment env)
        {
            _blobService = blobService;
            _env = env;
        }

        [HttpGet("Get")]
        public async Task<List<BlobModel>?> Get()
        {
            try
            {
                Log.Information("Retrieving URL data from blob storage container");
                return await _blobService.GetUrls();
            }
            catch (Exception ex)
            {
                Log.Error("GET Error: {0}", ex);
            }
            return null;
        }

        [HttpGet("GetByTag")]
        public async Task<List<BlobModel>?> GetByTag()
        {
            try
            {
                Log.Information("Retrieving URL data from blob storage container");
                return await _blobService.GetUrlsByTag();
            }
            catch (Exception ex)
            {
                Log.Error("GET Error: {0}", ex);
            }
            return null;
        }

        [HttpGet("GetByUserId/{id}")]
        public async Task<BlobModel?> GetByUserId(string id)
        {
            try
            {
                Log.Information("Retrieving URL data from blob storage container");
                var result = await _blobService.GetUrlsByTag();

                return result.Find(x => x.UserId == id);
            }
            catch (Exception ex)
            {
                Log.Error("GET Error: {0}", ex);
            }
            return null;
        }

        [HttpPost("UploadUserImage/{userId}")]
        public async Task Create(string userId)
        {
            Log.Information("Posting user image to blob storage");
            var formFile = Request.Form.Files[0];
            string trustedFileNameForFileStorage;
            var untrustedFileName = formFile.FileName;
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

            try
            {
                trustedFileNameForFileStorage = "tempUserImage.png";
                var path = Path.Combine(_env.ContentRootPath, "Development", "unsafe_uploads", trustedFileNameForFileStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await formFile.CopyToAsync(fs);

                Log.Information("{FileName} saved at {Path}", trustedFileNameForDisplay, path);
                fs.Close();

                await _blobService.UploadUserImageToContainer(userId);
            }
            catch (IOException ex)
            {
                Log.Error("{FileName} error on upload (Err: 3): {Message}", trustedFileNameForDisplay, ex.Message);
            }
        }
    }
}
