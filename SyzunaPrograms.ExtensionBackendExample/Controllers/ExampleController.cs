using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SyzunaPrograms.ExtensionBackendExample.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller")]
    // All routes in this controller need to be authenticated with the Bearer Authentication Scheme
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExampleController : ControllerBase
    {
        [HttpGet]
        // We do not override the controller scope authentication settings here because we want everybody with a valid JWT access this endpoint
        public ActionResult<string> ForEveryViewer()
        {
            return "Hello World! Welcome to the root example endpoint";
        }

        [HttpGet("/broadcaster")]
        // Overrides the controller scope authentication settings and restricts it to broadcaster only
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "requiresBroadcasterPermission")]
        public ActionResult<string> OnlyForBroadcastersEyes()
        {
            return "Hello World! Welcome to the broadcaster endpoint";
        }

        [HttpGet("/moderator")]
        // Overrides the controller scope authentication settings and restricts it to moderator and broadcaster only
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "requiresModeratorPermission")]
        public ActionResult<string> OnlyForAtLeastModeratorEyes()
        {
            return "Hello World! Welcome to the moderator endpoint. Or maybe you are the broadcaster?";
        }
    }
}