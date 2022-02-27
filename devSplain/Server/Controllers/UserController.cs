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
    public class UserController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;

        public UserController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet("Get")]
        public async Task<List<UserModel>?> Get()
        {
            try
            {
                Log.Information("Retrieving User data from container");
                return await _cosmosDbService.GetItemsAsync<UserModel>();
            }
            catch (Exception ex)
            {
                Log.Error("GET ERROR: {0}", ex);
            }

            return null;
        }

        [HttpGet("GetUser/{id}")]
        public async Task<UserModel?> GetUser(string id)
        {
            try
            {
                Log.Information("Retrieving User data from container");
                return await _cosmosDbService.GetItemAsync<UserModel>(id);
            }
            catch (Exception ex)
            {
                Log.Error("GET ERROR: {0}", ex);
            }

            return null;
        }

        [HttpGet("GetUserByAuthId/{authId}")]
        public async Task<ActionResult> GetUserByAuthId(string authId)
        {
            try
            {
                Log.Information("Retrieving User data from container");
                var result = await _cosmosDbService.GetItemByAuthIdAsync<UserModel>(authId);
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                Log.Error("GET ERROR: {0}", ex);
            }

            return BadRequest();
        }

        [HttpPost("Create")]
        public async Task<ActionResult> CreateAsync([Bind("UserId,FullName,FirstName,LastName,Gender,EmailAddress,Username,City,State,IsMD,IsAppAuthorized,IsEOSAvailable,IsRep,IsUserActive,UseBiometrics,TimeStamp,BlobURL,AuthId")] UserModel user)
        {
            if (ModelState.IsValid)
            {
                user.UserId = Guid.NewGuid().ToString();
                try
                {
                    await _cosmosDbService.AddItemAsync(user, user.UserId);
                    Log.Information("Created new user in the container: " + user.ToString());
                    return StatusCode(200);
                }
                catch (Exception ex)
                {
                    Log.Error("POST ERROR: {0}", ex);
                }
            }

            return BadRequest();
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> EditAsync([Bind("UserId,FullName,FirstName,LastName,Gender,EmailAddress,Username,City,State,IsMD,IsAppAuthorized,IsEOSAvailable,IsRep,IsUserActive,UseBiometrics,TimeStamp,BlobURL, AuthId")] UserModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _cosmosDbService.UpdateItemAsync(user.UserId, user);
                    Log.Information("Updated user in container.");
                    return StatusCode(200);
                }
                catch (Exception ex)
                {
                    Log.Error("POST ERROR: {0}", ex);
                }
            }

            return BadRequest();
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            try
            {
                await _cosmosDbService.DeleteItemAsync(id);
                Log.Information("Deleted user from the container");
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                Log.Error("GET ERROR: {0}", ex);
            }

            return BadRequest();
        }
    }
}
