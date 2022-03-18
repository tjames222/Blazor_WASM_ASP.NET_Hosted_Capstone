using devSplain.Server.Services;
using devSplain.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace devSplain.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;

        public PostController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpPost("Create")]
        public async Task<ActionResult> CreateAsync([Bind("PostId,Author,AuthorId,Title,Text,Image,Tag,CreatedOn,Type,Rating")] PostModel post)
        {
            if (ModelState.IsValid)
            {
                post.PostId = Guid.NewGuid().ToString();
                try
                {
                    await _cosmosDbService.AddItemAsync(post, post.PostId);
                    Log.Information("Created new Post in the container: " + post.Title.ToString());

                    return StatusCode(200);
                }
                catch (Exception ex)
                {
                    Log.Error("POST ERROR: {0}", ex);

                    return StatusCode(500);
                }
            }

            return BadRequest();
        }

        [HttpGet("Get/{id}")]
        public async Task<ActionResult> GetAsync(string id)
        {
            try
            {
                Log.Information("Retrieving post data from container");
                var result = await _cosmosDbService.GetItemAsync<PostModel>(id);

                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                Log.Error("GET ERROR: {0}", ex);

                return BadRequest();
            }
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAllAsync()
        {
            try
            {
                Log.Information("Retrieving User data from container");
                List<PostModel> result = await _cosmosDbService.GetItemsAsync<PostModel>();

                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                Log.Error("GET ERROR: {0}", ex);

                return NotFound();
            }
        }

        [HttpPut("Edit")]
        public async Task<ActionResult> EditAsync([Bind("PostId,Author,AuthorId,Title,Text,Image,Tag,CreatedOn,Type,Rating")] PostModel post)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _cosmosDbService.UpdateItemAsync(post.PostId, post);
                    Log.Information("Updated post in container.");

                    return StatusCode(200);
                }
                catch (Exception ex)
                {
                    Log.Error("POST ERROR: {0}", ex);

                    return BadRequest();
                }
            }

            return BadRequest();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            try
            {
                await _cosmosDbService.DeleteItemAsync(id);
                Log.Information("Deleted post from the container");

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Log.Error("GET ERROR: {0}", ex);

                return NotFound();
            }
        }
    }
}
