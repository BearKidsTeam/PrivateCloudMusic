using System;
using Anemonis.AspNetCore.RequestDecompression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pcm.Proxy;

namespace Pcm
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRequestDecompression(o =>
            {
                o.Providers.Add<DeflateDecompressionProvider>();
                o.Providers.Add<GzipDecompressionProvider>();
                o.Providers.Add<BrotliDecompressionProvider>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var handler = new MessageHandler();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();
            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/http", async context => await handler.HandleHttp(context));
                endpoints.MapGet("/music/{id}", async context => await handler.HandleMusic(context));
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await handler.HandleWs(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
        }
    }
}
