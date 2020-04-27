using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using TodoApi.Models;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace TodoAPISwagger
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
            services.AddDbContext<TodoContext>(opt =>
              opt.UseInMemoryDatabase("TodoList"));
            services.AddControllers(confg =>
            {
                confg.ReturnHttpNotAcceptable = true;
            }).AddNewtonsoftJson(confg =>
             {
                 confg.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
             })
            .AddXmlDataContractSerializerFormatters();

            services.AddVersionedApiExplorer(setupAction =>
            {
                setupAction.GroupNameFormat = "'v'VV";
            });

            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
            });

            var apiVersionDescriptionProvider =
               services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            services.AddSwaggerGen(setup =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    setup.SwaggerDoc(
                        $"TodoApiSwagger{description.GroupName}",
                        new Microsoft.OpenApi.Models.OpenApiInfo()
                        {
                            Title = "Todo API",
                            Version = description.ApiVersion.ToString(),
                            Description = "Aplicación de ejemplo para usar en charla de GDG Córdoba Argentina",
                            Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                            {
                                Email = "fedehaust@gmail.com",
                                Name = "Federico Haustein",
                                Url = new Uri("https://www.federicohaustein.com")
                            },
                            License = new Microsoft.OpenApi.Models.OpenApiLicense()
                            {
                                Name = "MIT License",
                                Url = new Uri("https://opensource.org/licenses/MIT")
                            }
                        });
                }
                setup.DocInclusionPredicate((documentName, apiDescription) =>
                {
                    var actionApiVersionModel = apiDescription.ActionDescriptor
                    .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }

                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v =>
                        $"TodoApiSwaggerv{v.ToString()}" == documentName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v =>
                        $"TodoApiSwaggerv{v.ToString()}" == documentName);
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                setup.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger(); //https://localhost:5001/swagger/TodoApiSwagger/swagger.json

            app.UseSwaggerUI(setup =>
            {
                setup.InjectStylesheet("/custom.css");
                setup.InjectJavascript("/custom.js"); 
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    setup.SwaggerEndpoint($"/swagger/" +
                        $"TodoApiSwagger{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
                setup.RoutePrefix = "";
            });
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
