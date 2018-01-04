using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Middlewares;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WebApp.Repositories;
using Microsoft.EntityFrameworkCore.Design;
using WebApp.Entities;
using WebApp.Dtos;
using WebApp.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using NLog.Web;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Swashbuckle.AspNetCore.Swagger;

namespace WebApp
{


    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            env.ConfigureNLog("nlog.config");

            Configuration = builder.Build();           
        } 
        //  This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddDbContext<MyDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ISeedDataService, SeedDataService>();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new Info { Title = "Customer Info WebApi", Version = "v1" });
            });

            services.AddMvc(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                //options.InputFormatters.Add(new XmlSerializerInputFormatter());
                //options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Error);
            loggerFactory.AddNLog();
            app.AddNLogWeb();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/plain";
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorFeature != null)
                    {
                        var logger = loggerFactory.CreateLogger("Global exception logger");
                        logger.LogError(500, errorFeature.Error, errorFeature.Error.Message);
                    }

                    await context.Response.WriteAsync("There was an error");
                });
            });
          


            if (env.IsEnvironment("MyEnvironment"))
            {
                app.UseCustomeMiddleware();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            AutoMapper.Mapper.Initialize(mapper =>
            {
                mapper.CreateMap<Customer, CustomerDto>().ReverseMap();
                mapper.CreateMap<Customer, CustomerCreateDto>().ReverseMap();
                mapper.CreateMap<Customer, CustomerUpdateDto>().ReverseMap();
            });

            app.AddSeedData();

            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer info web api");
            });

            app.UseMvc();
        }
    }
}
