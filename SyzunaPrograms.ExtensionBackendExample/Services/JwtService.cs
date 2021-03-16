using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace SyzunaPrograms.ExtensionBackendExample.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _signingKey;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _signingKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Twitch:ExtensionSecret"]));
        }

        public string CreateExternalTwitchExtensionJwt(string channelId, Dictionary<string, string[]> pubsubPerms)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("channel_id", channelId),
                    new Claim("user_id", _configuration["Twitch:ExtensionOwnerId"]),
                    new Claim(ClaimTypes.Role, "external"),
                    new Claim("pubsub_perms", JsonSerializer.Serialize(pubsubPerms))
                }),
                Expires = DateTime.Now.AddMinutes(2),
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}