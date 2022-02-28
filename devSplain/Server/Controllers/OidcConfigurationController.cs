using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace devSplain.Server.Controllers
{
    public class OidcConfigurationController : Controller
    {
        public OidcConfigurationController(IClientRequestParametersProvider clientRequestParametersProvider)
        {
            ClientRequestParametersProvider = clientRequestParametersProvider;
        }

        public IClientRequestParametersProvider ClientRequestParametersProvider { get; }

        [HttpGet("_configuration/{clientId}")]
        public IActionResult? GetClientRequestParameters([FromRoute] string clientId)
        {
            Log.Information("Retrieving client request parameters.");
            try
            {
                var parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
                return Ok(parameters);
            }
            catch (Exception ex)
            {
                Log.Error("ERROR: {0}", ex);
            }
            return null;
        }
    }
}
