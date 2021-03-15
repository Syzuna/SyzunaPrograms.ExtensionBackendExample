using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyzunaPrograms.ExtensionBackendExample.Extensions;
using SyzunaPrograms.ExtensionBackendExample.Models;

namespace SyzunaPrograms.ExtensionBackendExample.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    // All routes in this controller need to be authenticated with the Bearer Authentication Scheme
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExampleController : ControllerBase
    {
        [HttpGet]
        // We do not override the controller scope authentication settings here because we want everybody with a valid JWT access this endpoint
        public ActionResult<TwitchJwtData> ForEveryViewer()
        {
            return HttpContext.User.ExtracTwitchJwtData();
        }

        [HttpGet("broadcaster")]
        // Overrides the controller scope authentication settings and restricts it to broadcaster only
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "requiresBroadcasterPermissions")]
        public ActionResult<TwitchJwtData> OnlyForBroadcastersEyes()
        {
            return HttpContext.User.ExtracTwitchJwtData();
        }

        [HttpGet("moderator")]
        // Overrides the controller scope authentication settings and restricts it to moderator and broadcaster only
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "requiresModeratorPermissions")]
        public ActionResult<TwitchJwtData> OnlyForAtLeastModeratorEyes()
        {
            return HttpContext.User.ExtracTwitchJwtData();
        }
    }
}