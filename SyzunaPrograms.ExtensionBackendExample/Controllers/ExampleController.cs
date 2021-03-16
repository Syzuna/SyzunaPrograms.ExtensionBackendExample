using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SyzunaPrograms.ExtensionBackendExample.Extensions;
using SyzunaPrograms.ExtensionBackendExample.Models;
using SyzunaPrograms.ExtensionBackendExample.Services;

namespace SyzunaPrograms.ExtensionBackendExample.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    // All routes in this controller need to be authenticated with the Bearer Authentication Scheme
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExampleController : ControllerBase
    {
        private readonly ExtensionPubsubService _extensionPubsubService;
        private readonly JwtService _jwtService;

        public ExampleController(ExtensionPubsubService extensionPubsubService, JwtService jwtService)
        {
            _extensionPubsubService = extensionPubsubService;
            _jwtService = jwtService;
        }

        [HttpGet("jwt")]
        // Override the controller scope authentication settings to allow unauthenticated calls (Just for showcasing! Do not do this in production!)
        [AllowAnonymous]
        public ActionResult<string> CreateJwt()
        {
            var perms = new Dictionary<string, string[]>
            {
                { "send", new []{ "*" } }
            };

            return _jwtService.CreateExternalTwitchExtensionJwt("102943601", perms);
        }

        [HttpGet]
        // We do not override the controller scope authentication settings here because we want everybody with a valid JWT access this endpoint
        public ActionResult<TwitchJwtData> ForEveryViewer()
        {
            return HttpContext.User.ExtractTwitchJwtData();
        }

        [HttpGet("broadcaster")]
        // Overrides the controller scope authentication settings and restricts it to broadcaster only
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "requiresBroadcasterPermissions")]
        public ActionResult<TwitchJwtData> OnlyForBroadcastersEyes()
        {
            return HttpContext.User.ExtractTwitchJwtData();
        }

        [HttpGet("moderator")]
        // Overrides the controller scope authentication settings and restricts it to moderator and broadcaster only
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "requiresModeratorPermissions")]
        public ActionResult<TwitchJwtData> OnlyForAtLeastModeratorEyes()
        {
            return HttpContext.User.ExtractTwitchJwtData();
        }

        [HttpPost("broadcast")]
        // Overrides the controller scope authentication settings and restricts it to moderator and broadcaster only
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "requiresModeratorPermissions")]
        public ActionResult EnqueuePubsubBroadcastMessage([FromBody] ExtensionPubsubMessage message)
        {
            if (_extensionPubsubService.EnqueuePubsubMessage(message))
                return Ok(message);

            return StatusCode(500);
        }
    }
}