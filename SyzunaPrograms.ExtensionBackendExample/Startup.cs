using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using SyzunaPrograms.ExtensionBackendExample.HttpClients;
using SyzunaPrograms.ExtensionBackendExample.Services;

namespace SyzunaPrograms.ExtensionBackendExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddCors();

            // Setup of the JWT Bearer authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // The extension secret is base64 encoded and has to be decoded before using it
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Configuration["Twitch:ExtensionSecret"]))
                    };
                });

            // This step is kinda optional. You do not need to setup policies for role checking and could do it manually in the endpoint executing method
            services.AddAuthorization(options =>
            {
                options.AddPolicy("requiresBroadcasterPermissions", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "broadcaster");
                });

                options.AddPolicy("requiresModeratorPermissions", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "broadcaster", "moderator");
                });
            });

            services.AddHttpClient<TwitchExtensionHttpClient>();

            services.AddSingleton<ExtensionPubsubService>();
            services.AddSingleton<JwtService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseHttpsRedirection();

            // Add CORS to the execution pipeline
            // This can be tweaked and more restrictive than it its in this example. e.g. you could only allow GET, POST and OPTIONS (OPTIONS is required for CORS)
            // AllowCredentials is required
            app.UseCors(options =>
            {
                options
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("https://localhost", $"https://{Configuration["Twitch:ExtensionId"]}.ext-twitch.tv");
            });

            app.UseRouting();

            // Add our previously setup authentication and authorization to the pipeline
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
