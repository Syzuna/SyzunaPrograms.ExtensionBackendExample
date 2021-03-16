using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using SyzunaPrograms.ExtensionBackendExample.Models;
using SyzunaPrograms.ExtensionBackendExample.Services;

namespace SyzunaPrograms.ExtensionBackendExample.HttpClients
{
    public class TwitchExtensionHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;

        public TwitchExtensionHttpClient(HttpClient httpClient, IConfiguration configuration, JwtService jwtService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.twitch.tv/extensions");

            _configuration = configuration;
            _jwtService = jwtService;
        }

        public async Task<bool> SendExtensionPubsubMessageAsync(ExtensionPubsubMessage message)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"message/{message.ChannelId}");

                var perms = new Dictionary<string, string[]>
                {
                    { "send", new[] {"*"} }
                };

                var token = _jwtService.CreateExternalTwitchExtensionJwt(message.ChannelId, perms);

                request.Headers.Add("Client-ID", _configuration["Twitch:ExtensionId"]);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                request.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    ContentType = "application/json",
                    Message = JsonSerializer.Serialize(message.Message),
                    message.Targets
                }));

                var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return response.StatusCode == HttpStatusCode.NoContent;
            }
            catch
            {
                return false;
            }
        }
    }
}