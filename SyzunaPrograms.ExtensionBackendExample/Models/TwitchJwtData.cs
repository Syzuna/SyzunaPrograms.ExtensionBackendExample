using System.Collections.Generic;

namespace SyzunaPrograms.ExtensionBackendExample.Models
{
    public class TwitchJwtData
    {
        public bool IsUnlinked { get; set; }
        public string OpaqueUserId { get; set; }
        public string UserId { get; set; }
        public string ChannelId { get; set; }
        public string Role { get; set; }
        public Dictionary<string, string[]> PubSubPermissions { get; set; }
        public int IssuedAt { get; set; }
        public int ExpiresAt { get; set; }
    }
}