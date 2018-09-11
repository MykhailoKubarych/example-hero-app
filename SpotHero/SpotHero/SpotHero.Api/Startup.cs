﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpotHero.Api.Middleware.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Routing;
using SpotHero.Api.Middleware.RouteConstraints;

namespace SpotHero.Api
{
	public class Startup
	{
		public Startup(IHostingEnvironment environment)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(environment.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();

			JsonConvert.DefaultSettings = () => new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
			};

		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.RegisterServices(Configuration);

			services.AddMvc(options =>
				{
					options.RespectBrowserAcceptHeader = true;
					options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
				}).AddXmlSerializerFormatters()
				.AddXmlDataContractSerializerFormatters()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<RouteOptions>(routeOptions =>
            {         
                routeOptions.ConstraintMap.Add("iso8601date", typeof(ISO8601DateTimeOffsetConstraint));
            });

            services.AddSwaggerGen(c =>
		    {
		        c.SwaggerDoc("v1", new Info
		        {
		            Version = "v1",
		            Title = "SpotHero API",
		            Description = "A simple example ASP.NET Core Web API for Jonah Ellman",
		            TermsOfService = "None",
		            Contact = new Contact
		            {
		                Name = "Mykhailo Kubarych",
		                Email = "mishanyakub21@gmail.com"
		            },
		            License = new License
		            {
		                Name = "Use under LICX",
		                Url = "https://example.com/license"
		            },                  
		        });
		        // Set the comments path for the Swagger JSON and UI.
		        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
		        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
		        c.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
		    app.UseStaticFiles();

			app.ExceptionMiddleware();
            app.UseMvcWithDefaultRoute();
		    app.UseSwagger();
            app.UseSwaggerUI(c =>
		    {
		        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpotHero API");
		        c.RoutePrefix = "swagger";
		    });

			app.DbSeed(Configuration);
		}
	}
}
