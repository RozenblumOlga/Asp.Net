using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Middleware
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var date = DateTime.Now;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMiddleware<TokenMiddleware>(date);

            app.Use((context, next) =>
            {
                var newDate = DateTime.Now;
                var result = newDate - date;
                context.Response.WriteAsync($"\nProgram was executed in {result.Milliseconds} milliseconds");
                return Task.Delay(0);
            });
        }
    }

    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
         public DateTime date;

        public TokenMiddleware(RequestDelegate next, DateTime date)
        {
            this._next = next;
            this.date = date;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var newTime = DateTime.Now;
            var result = newTime - date;
            await context.Response.WriteAsync($"Program was executed in {result.Milliseconds} milliseconds");
            await _next.Invoke(context);
        }
    }
}
