using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using SyzunaPrograms.ExtensionBackendExample.Models;

namespace SyzunaPrograms.ExtensionBackendExample.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static TwitchJwtData ExtracTwitchJwtData(this ClaimsPrincipal claimsPrincipal)
        {
            var isUnlinked = claimsPrincipal.FindFirst(x => x.Type == "is_unlinked");
            var opaqueUserId = claimsPrincipal.FindFirst(x => x.Type == "opaque_user_id");
            var userId = claimsPrincipal.FindFirst(x => x.Type == "user_id");
            var channelId = claimsPrincipal.FindFirst(x => x.Type == "channel_id");
            var role = claimsPrincipal.FindFirst(x => x.Type == ClaimTypes.Role);
            var pubsubPerms = claimsPrincipal.FindFirst(x => x.Type == "pubsub_perms");
            var exp = claimsPrincipal.FindFirst(x => x.Type == "exp");
            var iat = claimsPrincipal.FindFirst(x => x.Type == "iat");

            return new TwitchJwtData
            {
                IsUnlinked = Convert.ToBoolean(isUnlinked?.Value),
                OpaqueUserId = opaqueUserId?.Value,
                UserId = userId?.Value,
                ChannelId = channelId?.Value,
                Role = role?.Value,
                PubSubPermissions = JsonSerializer.Deserialize<Dictionary<string, string[]>>(pubsubPerms?.Value ?? ""),
                ExpiresAt = Convert.ToInt32(exp?.Value),
                IssuedAt = Convert.ToInt32(iat?.Value)
            };
        }
    }
}