using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Middleware.Middlewares;

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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Middleware", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Middleware v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //App.Run()

            //app.Run(async context=> Console.WriteLine("Middleware 1."));//run metodu kısa devreye sebep olur.Run metodundan sonraki metodlar çalışmaz.
            //app.Run(async context=> Console.WriteLine("Middleware 2."));

            //app.Use()

            // app.Use(async (context,next)=>{
            //     Console.WriteLine("Middleware 1. başladı.");
            //     await next.Invoke();
            //     Console.WriteLine("Middlewre 1 sonlandırılıyor.");
            // });
            //   app.Use(async (context,next)=>{
            //     Console.WriteLine("Middleware 2. başladı.");
            //     await next.Invoke();
            //     Console.WriteLine("Middlewre 2 sonlandırılıyor.");
            // });
            //   app.Use(async (context,next)=>{
            //     Console.WriteLine("Middleware 3. başladı.");
            //     await next.Invoke();
            //     Console.WriteLine("Middlewre 3 sonlandırılıyor.");
            // });
            app.UseHello();

             app.Use(async (context,next)=>{
                Console.WriteLine("Use middleware tetiklendi.");
                await next.Invoke();               
            });

            //app.Map()
            app.Map("/example" , internalApp=> internalApp.Run(async context=> {
                Console.WriteLine("/example midlleware tetiklendi");
                await context.Response.WriteAsync("/example midlleware tetiklendi");
            }));

            //app.MapWhen()
            app.MapWhen(x=>x.Request.Method=="GET", internalApp=>
                {internalApp.Run(async context=>{
                Console.WriteLine("MapWhen midlleware tetiklendi");
                await context.Response.WriteAsync("MapWhen midlleware tetiklendi");
                });
            });

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
